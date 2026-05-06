using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Potions;

public class TrueRegenPotion : MokouModPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HpLossVar(6), new PowerVar<RegenPower>(6)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(MokouModKeywords.Nonlethal), HoverTipFactory.FromPower<RegenPower>()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var nonLethal = target.CurrentHp - DynamicVars.HpLoss.BaseValue < 1 ? target.CurrentHp - 1 : DynamicVars.HpLoss.BaseValue;
        await CreatureCmd.Damage(choiceContext, target, nonLethal, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Owner.Creature);
        await PowerCmd.Apply<RegenPower>(choiceContext, target, DynamicVars["RegenPower"].BaseValue, Owner.Creature, null);
    }
}