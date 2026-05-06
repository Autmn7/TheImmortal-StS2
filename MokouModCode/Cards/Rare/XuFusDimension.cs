using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class XuFusDimension : MokouModCard
{
    public XuFusDimension() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new PowerVar<RekindlePower>(1), new DynamicVar("XuFusDimensionPower", 2M));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RekindlePower>(choiceContext, Owner.Creature, DynamicVars["RekindlePower"].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<XuFusDimensionPower>(choiceContext, Owner.Creature,
            DynamicVars["XuFusDimensionPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XuFusDimensionPower"].UpgradeValueBy(1M);
    }
}