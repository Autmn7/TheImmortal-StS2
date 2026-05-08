using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MokouMod.MokouModCode.Powers;

public class RecklessSacrificesReflectionPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack())
            return;
        var reflectAmount = result.UnblockedDamage + result.OverkillDamage;
        if (reflectAmount > 0)
            await CreatureCmd.Damage(choiceContext, dealer, reflectAmount * Amount, ValueProp.Unpowered, null, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
            return;
        await PowerCmd.Remove(this);
    }
}