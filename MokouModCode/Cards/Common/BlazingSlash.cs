using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class BlazingSlash : MokouModCard
{
    public BlazingSlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(6);
        WithVars(new PowerVar<BurnPower>(2), new IgniteVar(4M), new PowerVar<WeakPower>(1));
        WithKeywords(MokouModKeywords.Ignite);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackSweepKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash").Execute(choiceContext);
        foreach (var enemy in CombatState.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.5f));
        await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies,
            DynamicVars["BurnPower"].IntValue, Owner.Creature, this);
        if (IgniteActive)
            await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies,
                DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }
}