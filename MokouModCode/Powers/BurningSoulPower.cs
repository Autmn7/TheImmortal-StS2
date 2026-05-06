using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Powers;

public class BurningSoulPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyPowerAmountGiven(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return cardSource == null || cardSource.Owner != Owner.Player || cardSource is not Feather ||
               power is not BurnPower
            ? amount
            : amount + Amount;
    }
}