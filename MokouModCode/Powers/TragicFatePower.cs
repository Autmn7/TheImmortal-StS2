using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MokouMod.MokouModCode.Powers;

public class TragicFatePower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || Owner.IsDead)
            return;
        SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");
        VfxCmd.PlayOnCreature(Owner, "vfx/vfx_attack_lightning");
        await CreatureCmd.Kill(Owner);
        await PowerCmd.Remove(this);
    }
}