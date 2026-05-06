using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class HighTempFeathers : MokouModCard
{
    public HighTempFeathers() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new CardsVar(2), new DynamicVar("ExhaustUpTo", 2M), new IgniteVar(10));
        WithKeyword(MokouModKeywords.Ignite);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(Feather));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        if (IgniteActive)
        {
            var prefs =
                new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars["ExhaustUpTo"].IntValue);
            var cards = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
                null, this);
            if (cards.Any())
            {
                foreach (var card in cards)
                    await CardCmd.Exhaust(choiceContext, card);
                await Feather.CreateInHand(Owner, cards.Count(), CombatState);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1M);
        DynamicVars["Ignite"].UpgradeValueBy(-2M);
    }
}