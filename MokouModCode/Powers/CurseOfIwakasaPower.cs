using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace MokouMod.MokouModCode.Powers;

public class CurseOfIwakasaPower : MokouModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyPowerAmountGivenMultiplicative(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        return target == Owner && power.Type == PowerType.Debuff ? 2M : 1M;
    }
}