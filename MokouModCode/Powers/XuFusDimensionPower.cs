using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MokouMod.MokouModCode.Powers;

public class XuFusDimensionPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.HasPower<RekindlePower>())
            CreatureCmd.GainMaxHp(Owner, Amount);
        return Task.CompletedTask;
    }
}