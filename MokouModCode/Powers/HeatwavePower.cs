using MegaCrit.Sts2.Core.Entities.Powers;

namespace MokouMod.MokouModCode.Powers;

public class HeatwavePower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}