using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Relics;

public class ImmortalPlume : MokouModRelic
{
    public int _essence;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<RegenPower>(4), new PowerVar<RekindlePower>(2)];

    public override bool ShowCounter => true;

    public override int DisplayAmount => Essence;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RekindlePower>(),
        HoverTipFactory.FromPower<RegenPower>(),
        HoverTipFactory.FromKeyword(MokouModKeywords.Essence)
    ];

    [SavedProperty]
    public int Essence
    {
        get => _essence;
        set
        {
            AssertMutable();
            _essence = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterActEntered()
    {
        Flash();
        if (Owner.RunState.CurrentActIndex > 0)
            Essence++;
        return Task.CompletedTask;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return;
        Flash();
        await PowerCmd.Apply<RekindlePower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars["RekindlePower"].BaseValue, Owner.Creature, null);
        await PowerCmd.Apply<RegenPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars["RegenPower"].BaseValue, Owner.Creature, null);
    }
}