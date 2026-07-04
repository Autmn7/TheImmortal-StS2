using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MokouMod.MokouModCode.Cards;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace MokouMod.MokouModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MokouMod"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        Log.Info("[MokouMod] Init called");
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Log.Info("[MokouMod] Harmony PatchAll completed");
        ModHelper.SubscribeForCombatStateHooks("MokouMod", CombatHookSubscription);
    }

    public static IEnumerable<AbstractModel> CombatHookSubscription(CombatState state)
    {
        foreach (var player in state.Players)
        {
            if (player.Creature.CombatState == null)
                continue;

            if (player.Character is Character.MokouMod mokou) yield return mokou;
        }
    }

    [HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
    public static class EmberResetPatch
    {
        private static void Postfix()
        {
            var tracker = CombatManager.Instance.StateTracker;
            MokouKeywordStateRegistry.Clear(tracker);
        }
    }

    [HarmonyPatch(typeof(RegenPower), nameof(RegenPower.BeforeSideTurnEndEarly))]
    public static class RegenPowerCustomDecayPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(RegenPower __instance, PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants, ref Task __result)
        {
            if (__instance.Owner.Player?.GetRelic<RawLiver>() != null)
            {
                __result = ExecuteRegenWithoutDecay(__instance, participants);
                return false;
            }

            return true;
        }


        private static async Task ExecuteRegenWithoutDecay(RegenPower power, IEnumerable<Creature> participants)
        {
            if (!participants.Contains(power.Owner) || power.Owner.IsDead)
                return;
            power.Flash();
            await CreatureCmd.Heal(power.Owner, power.Amount);
        }
    }

    [HarmonyPatch(typeof(VigorPower), nameof(VigorPower.AfterAttack))]
    public static class VigorousRefundPatch
    {
        [HarmonyPostfix]
        public static async void RefundVigor(VigorPower __instance, PlayerChoiceContext choiceContext, AttackCommand command)
        {
            var internalData = __instance.GetInternalData<VigorPower.Data>();

            if (internalData == null || internalData.commandToModify != command || command.ModelSource is not CardModel { Enchantment: VigorousEnchantment } card)
                return;
            var amountSpent = internalData.amountWhenAttackStarted;

            var refund = amountSpent / 2;
            if (command.Attacker != null && command.Attacker.HasPower<ValiantHeartPower>())
                refund = amountSpent;

            if (refund <= 0)
                return;

            await PowerCmd.Apply<VigorPower>(choiceContext, card.Owner.Creature, refund, card.Owner.Creature, card);
        }
    }


    [HarmonyPatch(typeof(EnchantmentModel), "get_DynamicExtraCardText")]
    public static class VigorousTextPatch
    {
        [HarmonyPostfix]
        private static void RenderVigorousText(EnchantmentModel __instance, ref LocString __result)
        {
            if (__instance is not VigorousEnchantment)
                return;

            // Choose correct text
            if (__instance.Card.Type == CardType.Attack)
                __result = new LocString("enchantments", "MOKOUMOD-VIGOROUS_ENCHANTMENT.attackText");
            else
                __result = new LocString("enchantments", "MOKOUMOD-VIGOROUS_ENCHANTMENT.nonAttackText");

            // Re-apply dynamic variables
            __result.Add("Amount", __instance.Amount);

            __result.Add("TargetType", __instance.IsCanonical ? "None" : __instance.Card.TargetType.ToString());

            __instance.DynamicVars.AddTo(__result);
        }
    }


    [HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
    public static class CardGlowPatch
    {
        [HarmonyPostfix]
        private static void RenderGlow(NHandCardHolder __instance)
        {
            var card = __instance.CardNode?.Model;
            if (card is not MokouModCard mokouCard || card.Owner.PlayerCombatState == null) return;

            var igniteActive = MokouModCard.TriggeredIgnite(mokouCard, mokouCard.DynamicVars.TryGetValue("Ignite", out var v) ? v.IntValue : 0);
            var furyActive = MokouModCard.TriggeredFury(mokouCard);
            var emberActive = MokouModCard.TriggeredEmber(mokouCard);

            if (!igniteActive && !furyActive && !emberActive) return;

            var highlight = __instance.CardNode.CardHighlight;

            var activeCount =
                (igniteActive ? 1 : 0) +
                (furyActive ? 1 : 0) +
                (emberActive ? 1 : 0);

            if (mokouCard.CanPlay())
            {
                if (igniteActive)
                    highlight.Modulate = new Color(1.0f, 0.75f, 0.2f, 0.98f);
                else if (furyActive)
                    highlight.Modulate = new Color(0.8f, 0.15f, 0.15f, 0.98f);
                else if (emberActive) highlight.Modulate = new Color(0.6f, 0.2f, 0.9f, 0.98f);

                if (mokouCard.Id.Entry.Equals("MOKOUMOD-TRINITY_SHIELD") ||
                    mokouCard.Id.Entry.Equals("MOKOUMOD-RAGING_INFERNO"))
                {
                    if (activeCount == 3)
                        highlight.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.98f);
                    else if (activeCount == 2) highlight.Modulate = new Color(1.0f, 0.5f, 0.5f, 0.98f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
    internal class EssenceTransferPatch
    {
        [HarmonyPrefix]
        private static void Prefix(RelicModel original, RelicModel replace)
        {
            if (original is BurningPlume old &&
                replace is ImmortalPlume neu)
                neu.Essence = old.Essence;
        }
    }

    [HarmonyPatch(typeof(CardModel), "GetDescriptionForPile")]
    [HarmonyPatch([typeof(PileType), typeof(CardModel.DescriptionPreviewType), typeof(Creature)])]
    public static class HideFuelKeywordsPatch
    {
        private static string GetCustomCardText(CardKeyword keyword)
        {
            // 1. Match the JSON casing
            var key = keyword.ToString().ToUpperInvariant();

            // 2. Fetch the strings
            var title = new LocString("card_keywords", key + ".title").GetFormattedText();
            var period = new LocString("card_keywords", "PERIOD").GetRawText();

            // 3. Return exactly what CardKeywordExtensions.GetCardText() produces
            return $"[gold]{title}[/gold]{period}";
        }

        [HarmonyPostfix]
        private static void HideFuelKeywords(CardModel __instance, ref string __result)
        {
            if (__instance is not MokouModFuelCard) return;
            try
            {
                var retainSearch = GetCustomCardText(CardKeyword.Retain);
                var unplayableSearch = GetCustomCardText(CardKeyword.Unplayable);

                if (string.IsNullOrEmpty(__result)) return;

                // Remove the keyword text and the newline the engine joined with
                __result = __result.Replace(retainSearch + "\n", "");
                __result = __result.Replace(retainSearch, "");

                __result = __result.Replace(unplayableSearch + "\n", "");
                __result = __result.Replace(unplayableSearch, "");

                __result = __result.Trim('\n', '\r', ' ');
            }
            catch (Exception)
            {
                // Safety first
            }
        }
    }

    [HarmonyPatch(typeof(NHealthBar))]
    public class BurnHealthBarPatch
    {
        private static readonly string BurnBarName = "MokouBurnForeground";
        private static readonly Color BurnColor = new(1.0f, 0.7f, 0.12f);

        [HarmonyPatch(nameof(NHealthBar._Ready))]
        [HarmonyPostfix]
        public static void AddBurnBar(NHealthBar __instance)
        {
            var poisonBar = __instance._poisonForeground as NinePatchRect;
            if (poisonBar == null) return;

            // Duplicate poison bar (already correct overlay behaviour)
            var burnBar = (NinePatchRect)poisonBar.Duplicate();
            burnBar.Name = BurnBarName;
            burnBar.SelfModulate = BurnColor;
            burnBar.Visible = false;

            __instance._hpForegroundContainer.AddChild(burnBar);

            var hpIndex = __instance._hpForeground.GetIndex();
            __instance._hpForegroundContainer.MoveChild(burnBar, hpIndex);

            // 🔧 Force proper overlay behaviour (prevents width bugs)
            burnBar.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            burnBar.SetOffsetsPreset(Control.LayoutPreset.FullRect);
            burnBar.SizeFlagsHorizontal = (int)Control.SizeFlags.ShrinkBegin;
            burnBar.SizeFlagsVertical = (int)Control.SizeFlags.ShrinkBegin;

            burnBar.OffsetLeft = 0;
            burnBar.OffsetRight = 0;
        }

        [HarmonyPatch(nameof(NHealthBar.RefreshForeground))]
        [HarmonyPostfix]
        public static void UpdateBurnVisuals(NHealthBar __instance)
        {
            var creature = __instance._creature;
            var burnBar = __instance._hpForegroundContainer.GetNodeOrNull<NinePatchRect>((NodePath)BurnBarName);

            if (burnBar == null || creature.CurrentHp <= 0 || creature.HpDisplay.IsInfinite())
                return;

            var burnPower = creature.GetPower<BurnPower>();
            var rawBurn = burnPower?.Amount ?? 0;
            var burnDamage = GetEffectiveBurnDamage(creature, rawBurn);

            var poisonPower = creature.GetPower<PoisonPower>();
            var poisonDamage = poisonPower?.CalculateTotalDamageNextTurn() ?? 0;

            // Poison has priority (match vanilla behavior)
            if (poisonDamage >= creature.CurrentHp)
            {
                burnBar.Visible = false;
                return;
            }

            if (burnDamage > 0)
            {
                burnBar.Visible = true;

                var maxW = __instance.MaxFgWidth;
                var hpEnd = __instance.GetFgWidth(creature.CurrentHp) - maxW;

                var totalDamage = poisonDamage + burnDamage;

                if (totalDamage >= creature.CurrentHp)
                {
                    // LETHAL: size burn so it visually fills but still respects patch margins
                    var widthAfterPoison = __instance.GetFgWidth(creature.CurrentHp - poisonDamage);
                    var widthAfterBoth =
                        __instance.GetFgWidth(creature.CurrentHp - totalDamage); // usually GetFgWidth(0)
                    burnBar.OffsetRight = widthAfterPoison - maxW;
                    burnBar.OffsetLeft = Mathf.Max(0.0f, widthAfterBoth - burnBar.PatchMarginLeft);

                    __instance._hpForeground.Visible = false;
                }
                else
                {
                    // NORMAL OVERLAY
                    var widthAfterPoison = __instance.GetFgWidth(creature.CurrentHp - poisonDamage);
                    var widthAfterBoth = __instance.GetFgWidth(creature.CurrentHp - totalDamage);

                    burnBar.OffsetRight = widthAfterPoison - maxW;
                    burnBar.OffsetLeft = Mathf.Max(0.0f, widthAfterBoth - burnBar.PatchMarginLeft);

                    __instance._hpForeground.Visible = true;
                }
            }
            else
            {
                burnBar.Visible = false;
            }
        }

        private static int GetEffectiveBurnDamage(Creature creature, int rawBurn)
        {
            if (rawBurn <= 0)
                return 0;
            var hpLoss = rawBurn;
            if (creature.HasPower<FireProofPower>()) hpLoss = 1;
            else if (creature.HasPower<PhoenixFormPower>() && hpLoss > 3) hpLoss = 3;
            if (creature.Player?.GetRelic<FiremanHelmet>() != null)
                hpLoss = creature.CurrentHp - hpLoss < 1 ? creature.CurrentHp - 1 : hpLoss;
            return hpLoss;
        }
    }

    [HarmonyPatch(typeof(NHealthBar), nameof(NHealthBar.RefreshText))]
    public static class BurnHealthBarTextPatch
    {
        private static readonly Color BurnColor = new(1.0f, 0.7f, 0.12f);
        private static readonly Color BurnOutline = new(0.45f, 0.25f, 0.05f);

        [HarmonyPostfix]
        public static void UpdateLethalBurnText(NHealthBar __instance)
        {
            var creature = __instance._creature;
            if (creature.CurrentHp <= 0 || creature.HpDisplay.IsInfinite()) return;

            var burnPower = creature.GetPower<BurnPower>();
            var rawBurn = burnPower?.Amount ?? 0;
            var burnDamage = GetEffectiveBurnDamage(creature, rawBurn);

            var poisonPower = creature.GetPower<PoisonPower>();
            var poisonDamage = poisonPower?.CalculateTotalDamageNextTurn() ?? 0;

            // Respect poison priority
            if (poisonDamage >= creature.CurrentHp) return;

            if (burnDamage + poisonDamage >= creature.CurrentHp)
            {
                __instance._hpLabel.AddThemeColorOverride("font_color", BurnColor);
                __instance._hpLabel.AddThemeColorOverride("font_outline_color", BurnOutline);
            }
        }

        private static int GetEffectiveBurnDamage(Creature creature, int rawBurn)
        {
            if (rawBurn <= 0)
                return 0;
            var hpLoss = rawBurn;
            if (creature.HasPower<FireProofPower>()) hpLoss = 1;
            else if (creature.HasPower<PhoenixFormPower>() && hpLoss > 3) hpLoss = 3;
            if (creature.Player?.GetRelic<FiremanHelmet>() != null)
                hpLoss = creature.CurrentHp - hpLoss < 1 ? creature.CurrentHp - 1 : hpLoss;
            return hpLoss;
        }
    }

    [HarmonyPatch(typeof(NHealthBar))]
    public class BurnHealthBarHideOnDeathPatch
    {
        private static readonly string BurnBarName = "MokouBurnForeground";

        [HarmonyPatch(nameof(NHealthBar.RefreshValues))]
        [HarmonyPostfix]
        public static void HideBurnOnDeath(NHealthBar __instance)
        {
            var burnBar = __instance._hpForegroundContainer.GetNodeOrNull<NinePatchRect>((NodePath)BurnBarName);
            if (burnBar == null) return;

            var creature = __instance._creature;
            if (creature.CurrentHp <= 0 || creature.HpDisplay.IsInfinite()) burnBar.Visible = false;
        }
    }
}