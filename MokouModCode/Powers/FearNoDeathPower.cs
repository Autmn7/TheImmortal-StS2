using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MokouMod.MokouModCode.Powers;

public class FearNoDeathPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || CombatState.CurrentSide != Owner.Side)
            return Task.CompletedTask;
        GetInternalData<Data>().playedCards.Add(cardPlay.Card, 0);
        return Task.CompletedTask;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || CombatState.CurrentSide != Owner.Side)
            return;
        if (cardSource == null || !GetInternalData<Data>().playedCards.ContainsKey(cardSource))
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        else
            GetInternalData<Data>().playedCards[cardSource] += Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        int amount;
        if (cardPlay.Card.Owner.Creature != Owner ||
            !GetInternalData<Data>().playedCards.Remove(cardPlay.Card, out amount))
            return;
        await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> playedCards = new();
    }
}