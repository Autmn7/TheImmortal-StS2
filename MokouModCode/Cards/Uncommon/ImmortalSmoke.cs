using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class ImmortalSmoke : MokouModCard
{
    public ImmortalSmoke() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new CardsVar("Feathers", 1));
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(MokouModKeywords.Exhume);
        WithTip(MokouModKeywords.Fuel);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card =
            (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                PileType.Exhaust.GetPile(Owner).Cards
                    .Where(model => model.Type == CardType.Skill)
                    .ToList(),
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1)
            )).FirstOrDefault();
        if (card == null)
            return;
        await CardPileCmd.Add(card, PileType.Hand);
        if (card.Keywords.Contains(MokouModKeywords.Fuel))
            await Feather.CreateInHand(Owner, DynamicVars["Feathers"].IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Feathers"].UpgradeValueBy(1M);
    }
}