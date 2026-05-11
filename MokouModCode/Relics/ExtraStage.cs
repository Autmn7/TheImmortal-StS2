using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Relics;

public class ExtraStage : MokouModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RekindlePower>(1), new("ActiveTurn", 7M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RekindlePower>()];

    public override bool ShowCounter => DisplayAmount > -1;

    public override int DisplayAmount
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress)
                return -1;
            var intValue = DynamicVars["ActiveTurn"].IntValue;
            var roundNumber = Owner.Creature.CombatState.RoundNumber;
            return roundNumber >= intValue ? intValue : roundNumber;
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;
        InvokeDisplayAmountChanged();
        var intValue = DynamicVars["ActiveTurn"].IntValue;
        var roundNumber = Owner.Creature.CombatState.RoundNumber;
        if (roundNumber != intValue)
            return;
        Flash();
        await PowerCmd.Apply<RekindlePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["RekindlePower"].BaseValue, Owner.Creature,
            null);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}