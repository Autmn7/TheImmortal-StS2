using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Relics;
using static MokouMod.MokouModCode.Cards.MokouModCard;

namespace MokouMod.MokouModCode.Powers;

public class BurnPower : MokouModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task TriggerBurnEffect(CombatSide side, ICombatState combatState, CardModel? cardSource = null)
    {
        var hasFireProof = Owner.HasPower<FireProofPower>();
        var hpLoss = (decimal)Amount;
        if (hasFireProof) hpLoss = 1m;
        else if (Owner.HasPower<PhoenixFormPower>() && hpLoss > 3m) hpLoss = 3m;
        if (Owner.Player?.GetRelic<FiremanHelmet>() != null)
            hpLoss = CalculateNonLethal(Owner, hpLoss);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurningVfx.Create(Owner, Math.Min(2.0f, 0.5f + (float)(Math.Ceiling(Amount / 10.0) * 0.05f)), Owner.IsPlayer));
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, cardSource);
        if (Owner.IsAlive && (side == CombatSide.Player || CombatState.Players.All(player => !player.HasPower<FujiyamaVolcanoPower>())))
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -(int)Math.Ceiling(Amount / 5.0), null, null);
        if (hasFireProof)
            await PowerCmd.Decrement(Owner.GetPower<FireProofPower>());
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;
        await TriggerBurnEffect(side, combatState);
    }
}