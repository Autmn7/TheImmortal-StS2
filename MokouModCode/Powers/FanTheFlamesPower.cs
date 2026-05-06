using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Powers;

public class FanTheFlamesPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner)
            return;
        if (card.Keywords.Contains(MokouModKeywords.Fuel))
        {
            Flash();
            var enemy =
                Owner.Player.RunState.Rng.CombatTargets.NextItem(
                    CombatState.HittableEnemies);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.75f));
            await PowerCmd.Apply<BurnPower>(choiceContext, enemy, Amount, Owner, null);
        }
    }
}