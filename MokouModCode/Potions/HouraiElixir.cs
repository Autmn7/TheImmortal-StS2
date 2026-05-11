using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Potions;

public class HouraiElixir : MokouModPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.AnyTime;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Essence", 1), new PowerVar<RekindlePower>(1)];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MokouModKeywords.Essence), HoverTipFactory.FromPower<RekindlePower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner.GetRelic<ImmortalPlume>() != null)
        {
            Owner.GetRelic<ImmortalPlume>()!.Flash();
            Owner.GetRelic<ImmortalPlume>()!.Essence += DynamicVars["Essence"].IntValue;
        }
        else if (Owner.GetRelic<BurningPlume>() != null)
        {
            Owner.GetRelic<BurningPlume>()!.Flash();
            Owner.GetRelic<BurningPlume>()!.Essence += DynamicVars["Essence"].IntValue;
        }

        if (CombatManager.Instance.IsInProgress)
            await PowerCmd.Apply<RekindlePower>(choiceContext, target, DynamicVars["RekindlePower"].BaseValue, Owner.Creature, null);
    }
}