using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class BlazingBird : MokouModCard
{
    public BlazingBird() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVars(new CardsVar("Feathers", 1), new IgniteVar(3M), new EnergyVar(1));
        WithKeywords(MokouModKeywords.Ignite);
        WithTip(typeof(Feather));
        WithEnergyTip();
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellBackflip;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.5f));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(choiceContext);
        await Feather.CreateInHand(Owner, CombatState);
        if (IgniteActive)
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}