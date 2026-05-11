using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Relics;

public class OfudaOfIgnition : MokouModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnPower>(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BurnPower>()];

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            return;
        Flash();
        foreach (var enemy in Owner.Creature.CombatState.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.5f));
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature.CombatState.HittableEnemies, DynamicVars["BurnPower"].IntValue, Owner.Creature,
            null);
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature.CombatState.HittableEnemies, DynamicVars["BurnPower"].IntValue, Owner.Creature,
            null);
    }
}