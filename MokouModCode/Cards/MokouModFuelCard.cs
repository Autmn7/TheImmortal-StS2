using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MokouMod.MokouModCode.Cards;

public abstract class MokouModFuelCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    MokouModCard(cost, type, rarity, target)
{
    public decimal Durability;
    public decimal MaxDurability;
    public bool Triggered;

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
        if (Triggered)
        {
            await OnFuelTrigger();
            Durability--;
            if (Durability <= 0)
            {
                await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), this);
                await OnFuelDurabilityDeplete();
                Durability = MaxDurability;
            }
        }

        Triggered = false;
    }
}