using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class RagingInferno : MokouModCard
{
    public RagingInferno() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithVars(new PowerVar<WeakPower>(2), new PowerVar<VulnerablePower>(2), new IgniteVar(5M));
        WithKeywords(MokouModKeywords.Ignite, MokouModKeywords.Fury);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackAirKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.6f));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .Execute(choiceContext);
        if (IgniteActive)
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        if (FuryActive)
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}