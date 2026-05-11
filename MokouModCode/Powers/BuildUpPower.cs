using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MokouMod.MokouModCode.Powers;

public class BuildUpPower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override object InitInternalData() => (object) new Data();

    public override Task BeforeAttack(AttackCommand command)
    {
        if (!(command.ModelSource is CardModel modelSource) || modelSource.Owner.Creature != this.Owner || modelSource.Type != CardType.Attack || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        Data internalData = this.GetInternalData<Data>();
        if (internalData.commandToModify != null)
            return Task.CompletedTask;
        internalData.commandToModify = command;
        return Task.CompletedTask;
    }

    public override Decimal ModifyDamageMultiplicative(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource == null || cardSource.Owner.Creature != this.Owner || !props.IsPoweredAttack())
            return 1M;
        Data internalData = this.GetInternalData<Data>();
        return internalData.commandToModify != null && cardSource != internalData.commandToModify.ModelSource ? 1M : 2M;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Data internalData = GetInternalData<Data>();
        if (command != internalData.commandToModify)
            return;
        internalData.commandToModify = (AttackCommand) null;
        await PowerCmd.Decrement((PowerModel) this);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        await PowerCmd.Remove((PowerModel) this);
    }

    public class Data
    {
        public AttackCommand? commandToModify;
    }
}