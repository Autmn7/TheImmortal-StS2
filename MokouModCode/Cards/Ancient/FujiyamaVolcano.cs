using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Ancient;

public class FujiyamaVolcano : MokouModCard
{
    public FujiyamaVolcano() : base(1, CardType.Power, CardRarity.Ancient, TargetType.RandomEnemy)
    {
        WithVars(new PowerVar<BurnPower>(1), new RepeatVar(6));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FujiyamaVolcanoPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurningVfx.Create(Owner.Creature, 1f, false));
        await Cmd.CustomScaledWait(0.2f, 0.3f);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        for (var i = 0; i < DynamicVars.Repeat.IntValue; ++i)
        {
            var enemy =
                Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            if (enemy != null)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.75f));
                await CommonActions.Apply<BurnPower>(enemy, this, DynamicVars["BurnPower"].IntValue);
                await Cmd.CustomScaledWait(0.1f, 0.15f);
            }

            enemy = null;
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}