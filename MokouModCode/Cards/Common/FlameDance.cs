using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Cards.Common;

public class FlameDance : MokouModCard
{
    public FlameDance() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVars(new CardsVar("Feathers", 3));
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Feather));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Feather.CreateInHand(Owner, DynamicVars["Feathers"].IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Feathers"].UpgradeValueBy(1M);
    }
}