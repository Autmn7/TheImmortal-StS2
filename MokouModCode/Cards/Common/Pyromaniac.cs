using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Common;

public class Pyromaniac : MokouModCard
{
    public Pyromaniac() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8);
        WithVars(new PowerVar<BurnPower>(2));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellCast;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.75f));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}