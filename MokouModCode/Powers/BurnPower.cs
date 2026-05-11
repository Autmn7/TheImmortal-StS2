using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Relics;

namespace MokouMod.MokouModCode.Powers;

public class BurnPower : MokouModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task TriggerBurnEffect(CombatSide side, ICombatState combatState, CardModel? cardSource = null)
    {
        var hasFireProof = Owner.HasPower<FireProofPower>();
        var hpLoss = Amount;
        if (Owner.HasPower<FireProofPower>()) hpLoss = 1;
        else if (Owner.HasPower<PhoenixFormPower>() && hpLoss > 3) hpLoss = 3;
        if (Owner.Player?.GetRelic<FiremanHelmet>() != null)
            hpLoss = Owner.CurrentHp - hpLoss < 1 ? Owner.CurrentHp - 1 : hpLoss;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurningVfx.Create(Owner,
            Math.Min(2.0f, 0.5f + (float)(Math.Ceiling(Amount / 10.0) * 0.05f)), Owner.IsPlayer)!);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, cardSource);
        if (Owner.IsAlive && (side == CombatSide.Player || CombatState.Players.All(player => !player.HasPower<FujiyamaVolcanoPower>())))
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -(int)Math.Ceiling(Amount / 5.0), null, null);
        if (hasFireProof)
            await PowerCmd.Decrement(Owner.GetPower<FireProofPower>()!);
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;
        await TriggerBurnEffect(side, combatState);
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource != null)
            if (cardSource.Id.Entry.Equals("MOKOUMOD-PYROMANIAC") ||
                cardSource.Id.Entry.Equals("MOKOUMOD-FLAME_WHIRLWIND"))
                return Owner != dealer || !props.IsPoweredAttack() ? 0M : Amount;

        return 0M;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != null)
            if (cardSource.Owner.Creature == Owner && cardSource.Id.Entry.Equals("MOKOUMOD-FIRE_PROTECTION"))
                return !props.IsPoweredCardOrMonsterMoveBlock() ? 0M : Amount;

        return 0M;
    }
}