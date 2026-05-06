using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MokouMod.MokouModCode.Powers;

public class CrimsonWatchguardPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return Task.CompletedTask;
        GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, Amount);
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount;
        var enchanted = cardPlay.Card.Enchantment;
        if (cardPlay.Card.Owner.Creature != Owner || !GetInternalData<Data>()
                .amountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
            return;
        if (cardPlay.Card.Type == CardType.Skill)
        {
            Flash();
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner, enchanted == null ? Amount : Amount + 1, Owner,
                null);
        }
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> amountsForPlayedCards = new();
    }
}