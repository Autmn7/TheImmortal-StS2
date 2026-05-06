using MegaCrit.Sts2.Core.Entities.Powers;

namespace MokouMod.MokouModCode.Powers;

public class FireProofPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}