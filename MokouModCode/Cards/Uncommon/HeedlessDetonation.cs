using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class HeedlessDetonation : MokouModCard
{
    public HeedlessDetonation() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithVars(new PowerVar<BurnPower>(5), new DynamicVar("Ratio", 1M));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
        if (Owner.Creature.HasPower<BurnPower>())
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                var creatureNode = NCombatRoom.Instance?.GetCreatureNode(enemy);
                if (GodotObject.IsInstanceValid(creatureNode))
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(enemy));
            }

            await Cmd.CustomScaledWait(0.2f, 0.3f);
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies,
                Owner.Creature.GetPowerAmount<BurnPower>() * DynamicVars["Ratio"].IntValue,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Ratio"].UpgradeValueBy(1M);
    }
}