using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class WingBlast : MokouModCard
{
    public WingBlast() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(7);
        WithVars(new EnergyVar(2));
        WithKeywords(MokouModKeywords.Fury, MokouModKeywords.Ember);
        WithEnergyTip();
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitCount(2).WithHitFx("vfx/vfx_giant_horizontal_slash").Execute(choiceContext);
        if (FuryActive || EmberActive)
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue,
                Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}