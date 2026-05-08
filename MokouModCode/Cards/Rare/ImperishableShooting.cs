using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class ImperishableShooting : MokouModCard
{
    public ImperishableShooting() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(8);
        WithVars(new PowerVar<BurnPower>(4M));
        WithKeyword(CardKeyword.Exhaust);
        WithTip(typeof(RekindlePower));
        WithTip(MokouModKeywords.Exhume);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .BeforeDamage(async () =>
            {
                var targetCenterPositions = new Array<Vector2>();
                foreach (var enemy in CombatState.HittableEnemies)
                {
                    var creatureNode2 = NCombatRoom.Instance?.GetCreatureNode(enemy);
                    if (creatureNode2 != null)
                        targetCenterPositions.Add(creatureNode2.VfxSpawnPosition);
                }

                var vfx = NSweepingBeamVfx.Create(new Vector2(500, 520), targetCenterPositions);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                await Cmd.CustomScaledWait(0.5f, 0.75f);
            }).Execute(choiceContext);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.75f));
        }

        await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }
}