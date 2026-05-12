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
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;
using MethodInfo = System.Reflection.MethodInfo;

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

    [HarmonyPatch(typeof(VigorPower), nameof(VigorPower.AfterAttack))]
    public static class VigorPower_VigorousRefundPatch
    {
        private static readonly Type dataType = AccessTools.Inner(typeof(VigorPower), "Data");

        private static readonly MethodInfo getInternalDataMethod =
            AccessTools.Method(typeof(VigorPower), "GetInternalData").MakeGenericMethod(dataType);

        private static readonly FieldInfo commandField =
            AccessTools.Field(dataType, "commandToModify");

        private static readonly FieldInfo amountField =
            AccessTools.Field(dataType, "amountWhenAttackStarted");

        private static async void Postfix(VigorPower __instance, AttackCommand command)
        {
            // Get internal data via reflection
            var internalData = getInternalDataMethod.Invoke(__instance, null);

            if (internalData == null)
                return;
            var storedCommand = commandField.GetValue(internalData) as AttackCommand;

            if (storedCommand != command)
                return;

            // Get the card source
            var card = command.ModelSource as CardModel;
            if (card == null)
                return;

            // Check for VigorousEnchantment
            var hasVigorous = HasVigorousEnchantment(card);

            if (!hasVigorous)
                return;

            var amountSpent = (int)amountField.GetValue(internalData);

            // Refund
            var refund = amountSpent / 2;
            if (__instance.Owner.HasPower<ValiantHeartPower>())
                refund = amountSpent;

            if (refund <= 0)
                return;

            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), card.Owner.Creature, refund,
                card.Owner.Creature, card);
        }

        private static bool HasVigorousEnchantment(CardModel card)
        {
            return card.Enchantment != null &&
                   card.Enchantment.Id.Entry.Equals("MOKOUMOD-VIGOROUS_ENCHANTMENT");
        }
    }


    [HarmonyPatch(typeof(EnchantmentModel), "get_DynamicExtraCardText")]
    public static class VigorousEnchantment_TextPatch
    {
        private static void Postfix(EnchantmentModel __instance, ref LocString __result)
        {
            if (__instance.Id?.Entry != "MOKOUMOD-VIGOROUS_ENCHANTMENT")
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
        private static void Postfix(NHandCardHolder __instance)
        {
            var card = __instance.CardNode?.Model;
            if (card == null || card is not MokouModCard mokouCard || card.Owner.PlayerCombatState == null) return;

            var igniteActive = MokouModCard.TriggeredIgnite(mokouCard,
                mokouCard.DynamicVars.TryGetValue("Ignite", out var v) ? v.IntValue : 0);
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
    [HarmonyPatch(new[] { typeof(PileType), typeof(CardModel.DescriptionPreviewType), typeof(Creature) })]
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

        private static void Postfix(CardModel __instance, ref string __result)
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

    [HarmonyPatch(typeof(PowerCmd), "Apply", typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature),
        typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool))]
    public static class MarkOfSinPreventionPatch
    {
        private static bool Prefix(PowerModel power, Creature target, Creature? applier, ref Task __result)
        {
            if (power is MarkOfSinPower)
            {
                var alreadyMarkedByMe = target.Powers.Any(p =>
                    p is MarkOfSinPower existing &&
                    existing.Applier == applier);
                if (alreadyMarkedByMe)
                {
                    __result = Task.CompletedTask;
                    return false;
                }
            }

            return true;
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
            var rawBurn = burnPower != null ? burnPower.Amount : 0;
            var burnDamage = GetEffectiveBurnDamage(creature, rawBurn);

            var poisonPower = creature.GetPower<PoisonPower>();
            var poisonDamage = poisonPower != null ? poisonPower.CalculateTotalDamageNextTurn() : 0;

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
            var rawBurn = burnPower != null ? burnPower.Amount : 0;
            var burnDamage = GetEffectiveBurnDamage(creature, rawBurn);

            var poisonPower = creature.GetPower<PoisonPower>();
            var poisonDamage = poisonPower != null ? poisonPower.CalculateTotalDamageNextTurn() : 0;

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