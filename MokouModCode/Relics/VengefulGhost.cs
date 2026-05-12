using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MokouMod.MokouModCode.Relics;

public class VengefulGhost : MokouModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (target != Owner.Creature || result.UnblockedDamage <= 0 || Owner.Creature.CombatState.CurrentSide != Owner.Creature.Side)
            return;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, result.UnblockedDamage,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }
}