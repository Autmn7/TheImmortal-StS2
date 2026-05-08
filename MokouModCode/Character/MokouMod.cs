using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Cards;
using MokouMod.MokouModCode.Cards.Basic;
using MokouMod.MokouModCode.Extensions;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Character;

public class MokouMod : PlaceholderCharacterModel
{
    public enum Animation
    {
        None,
        AttackCloseHeavy,
        AttackCloseLight,
        AttackCloseRound,
        AttackDashHeavy,
        AttackDashLight,
        Block,
        Broken,
        DamageHeavy,
        DamageLight,
        Dead,
        Guard,
        Idle,
        ShotA,
        ShotB,
        ShotC,
        Resurrection,
        ShotLoop,
        SpellFastA,
        SpellLongA,
        SpellLongB,
        StartBattle,
        Stun,
        StunEnd,
        Win
    }

    public const string CharacterId = "MokouMod";

    public static readonly Color Color = new("#c81e1e");

    public override Color NameColor => Color;
    public override Color EnergyLabelOutlineColor => new("#8b0000");
    public override Color MapDrawingColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 50;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeMokou>(),
        ModelDb.Card<StrikeMokou>(),
        ModelDb.Card<StrikeMokou>(),
        ModelDb.Card<StrikeMokou>(),
        ModelDb.Card<DefendMokou>(),
        ModelDb.Card<DefendMokou>(),
        ModelDb.Card<DefendMokou>(),
        ModelDb.Card<DefendMokou>(),
        ModelDb.Card<SelfHarmingKick>(),
        ModelDb.Card<BreakTime>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BurningPlume>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<MokouModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MokouModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MokouModPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomVisualPath => "visual/character_visual_mokou.tscn".ScenePath();
    public override string CustomCharacterSelectBg => "select/character_select_bg_mokou.tscn".ScenePath();
    public override string CustomEnergyCounterPath => "energy/energy_counter_mokou.tscn".ScenePath();
    public override string CustomRestSiteAnimPath => "rest/rest_site_mokou.tscn".ScenePath();
    public override string CustomMerchantAnimPath => "merchant/merchant_mokou.tscn".ScenePath();

    public override string CustomIconTexturePath => "character_icon_mokou.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_outline_mokou.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_mokou.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_mokou_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_mokou.png".CharacterUiPath();

    public override string CustomArmPointingTexturePath => "hand_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "hand_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "hand_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "hand_scissors.png".CharacterUiPath();

    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";

    //public override string CustomAttackSfx => null;
    //public override string CustomCastSfx => null;
    //public override string CustomDeathSfx => null;
    //public override string CharacterSelectSfx => null;
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";


    public static void RunAnimation(Player player, Animation animation)
    {
        var instance = NCombatRoom.Instance;
        var val = instance != null ? instance.GetCreatureNode(player.Creature) : null;
        if (val != null && val.Visuals is MokouVisuals visuals)
        {
            var playback = visuals.Playback;
            if (playback != null) playback.Travel((StringName)animation.ToString(), true);
        }
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Character is MokouMod)
        {
            var instance = NCombatRoom.Instance;
            var val = instance != null ? instance.GetCreatureNode(cardPlay.Card.Owner.Creature) : null;
            if (val != null && val.Visuals is MokouVisuals { Playback: var playback })
            {
                if (cardPlay.Card is MokouModCard { Animation: not Animation.None } mokouModCard)
                {
                    if (playback != null) playback.Travel((StringName)mokouModCard.Animation.ToString(), true);
                    return Task.CompletedTask;
                }

                DefaultCardAnimation(cardPlay, playback);
            }
        }
        return base.BeforeCardPlayed(cardPlay);
    }

    public virtual void DefaultCardAnimation(CardPlay cardPlay, AnimationNodeStateMachinePlayback playback)
    {
        var card = cardPlay.Card;
        var animation = Animation.None;
        switch (card.Type)
        {
            case CardType.Attack:
                animation = card.DynamicVars.ContainsKey("Damage") && ((DynamicVar)card.DynamicVars.Damage).IntValue >= 20
                    ? Animation.AttackCloseHeavy
                    : Animation.AttackCloseRound;
                break;
            case CardType.Skill:
                animation = !card.DynamicVars.ContainsKey("Block") || !(((DynamicVar)card.DynamicVars.Block).BaseValue > 1m)
                    ? Animation.SpellFastA
                    : Animation.Block;
                break;
            case CardType.Power:
            {
                animation = Animation.SpellLongA;
                break;
            }
        }
        playback.Travel((StringName)animation.ToString(), true);
    }

    public override Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != dealer && target.IsPlayer)
        {
            var player = target.Player;
            if ((player != null ? player.Character : null) is MokouMod)
            {
                var instance = NCombatRoom.Instance;
                var val = instance != null ? instance.GetCreatureNode(target) : null;
                if (val != null && val.Visuals is MokouVisuals { Playback: var playback })
                {
                    if (result.WasFullyBlocked)
                    {
                        if (playback != null) playback.Travel((StringName)nameof(Animation.Guard), true);
                    }
                    else if (result.WasBlockBroken && result.UnblockedDamage > 0)
                    {
                        if (playback != null) playback.Travel((StringName)nameof(Animation.Broken), true);
                    }
                    else if (!((Enum)result.Props).HasFlag((Enum)(object)(ValueProp)16) && PublicPropExtensions.IsCardOrMonsterMove_(result.Props))
                    {
                        if ((float)result.UnblockedDamage > (float)target.CurrentHp * 0.25f)
                        {
                            if (playback != null) playback.Travel((StringName)nameof(Animation.DamageHeavy), true);
                        }
                        else if (playback != null)
                        {
                            playback.Travel((StringName)nameof(Animation.DamageLight), true);
                        }
                    }
                }
            }
        }
        return base.AfterDamageReceivedLate(choiceContext, target, result, props, dealer, cardSource);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!wasRemovalPrevented && creature.IsPlayer)
        {
            var player = creature.Player;
            if (player?.Character is MokouMod) RunAnimation(player, Animation.Dead);
        }
        return base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        foreach (var player in room.CombatState.Players)
            if (player.Character is MokouMod)
                RunAnimation(player, Animation.Win);
        return base.AfterCombatVictory(room);
    }
}