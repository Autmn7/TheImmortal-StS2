using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class LegendOfImmortality : MokouModCard
{
    public LegendOfImmortality() : base(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<RekindlePower>(1), new PowerVar<TempEssencePower>(1), new DynamicVar("LegendOfImmortalityPower", 1M));
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RekindlePower>(choiceContext, Owner.Creature, DynamicVars["RekindlePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<TempEssencePower>(choiceContext, Owner.Creature, DynamicVars["TempEssencePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<LegendOfImmortalityPower>(choiceContext, Owner.Creature, DynamicVars["LegendOfImmortalityPower"].BaseValue, Owner.Creature, this);
    }
}