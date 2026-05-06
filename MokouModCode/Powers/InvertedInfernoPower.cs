using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MokouMod.MokouModCode.Powers;

public class InvertedInfernoPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        var cardsToExhaust =
            await CardSelectCmd.FromHand(
                choiceContext,
                player,
                new CardSelectorPrefs(Description, 0, Amount),
                null,
                this
            );
        if (cardsToExhaust.Any())
        {
            foreach (var cardA in cardsToExhaust)
                await CardCmd.Exhaust(choiceContext, cardA);
            var cardsToRetrieve =
                await CardSelectCmd.FromSimpleGrid(
                    choiceContext,
                    PileType.Discard.GetPile(Owner.Player).Cards,
                    Owner.Player,
                    new CardSelectorPrefs(SelectionScreenPrompt, cardsToExhaust.Count())
                );
            foreach (var cardB in cardsToRetrieve)
                await CardPileCmd.Add(cardB, PileType.Hand);
        }
    }
}