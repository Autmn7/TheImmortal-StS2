using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Basic;
using MokouMod.MokouModCode.Extensions;
using MokouMod.MokouModCode.Relics;

namespace MokouMod.MokouModCode.Character;

public class MokouMod : PlaceholderCharacterModel
{
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

    public override string CustomVisualPath => "character_visual_mokou.tscn".CharacterUiPath();
    public override string CustomCharacterSelectBg => "character_select_bg_mokou.tscn".CharacterUiPath();
    public override string CustomIconTexturePath => "character_icon_mokou.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "energy_counter_mokou.tscn".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_mokou.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_mokou_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_mokou.png".CharacterUiPath();

    public override string CustomRestSiteAnimPath => "campfire_mokou.tscn".CharacterUiPath();
    public override string CustomMerchantAnimPath => "shop_mokou.tscn".CharacterUiPath();
    public override string CustomArmPointingTexturePath => "hand_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "hand_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "hand_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "hand_scissors.png".CharacterUiPath();

    public override string CustomCharacterSelectTransitionPath =>
        "res://materials/transitions/ironclad_transition_mat.tres";

    //public override string CustomAttackSfx => null;
    //public override string CustomCastSfx => null;
    //public override string CustomDeathSfx => null;
    //public override string CharacterSelectSfx => null;
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
}