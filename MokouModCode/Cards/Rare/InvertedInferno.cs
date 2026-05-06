using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class InvertedInferno : MokouModCard
{
    public InvertedInferno() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new DynamicVar("InvertedInfernoPower", 1M));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InvertedInfernoPower>(choiceContext, Owner.Creature,
            DynamicVars["InvertedInfernoPower"].BaseValue, Owner.Creature, this);
    }
}