using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards;

public abstract class MokouModFuelCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    MokouModCard(cost, type, rarity, target)
{
    public decimal Durability;
    public decimal MaxDurability;

    protected virtual Task OnFuelTrigger()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnFuelDurabilityDeplete()
    {
        return Task.CompletedTask;
    }

    public async Task TriggerFuel(Player player)
    {
        if (player != Owner) return;

        // This method is now safely invoked only by validated context snapshots
        await OnFuelTrigger();
        Durability--;
        if (Durability <= 0)
        {
            await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), this);
            await OnFuelDurabilityDeplete();
            Durability = MaxDurability;
        }
    }
    
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == Owner.Creature && power is FanTheFlamesPower)
        {
            CardCmd.RemoveKeyword(this, CardKeyword.Unplayable);
            CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
            EnergyCost.SetThisCombat(0);
        }

        base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        return Task.CompletedTask;
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone || !Owner.Creature.HasPower<FanTheFlamesPower>())
            return Task.CompletedTask;
        CardCmd.RemoveKeyword(this, CardKeyword.Unplayable);
        CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
        EnergyCost.SetThisCombat(0);
        return Task.CompletedTask;
    }
}