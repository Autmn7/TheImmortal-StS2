using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class FinalTalon : MokouModCard
{
    public FinalTalon() : base(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
    {
        WithDamage(8);
        WithVars(new PowerVar<BurnPower>(4));
        WithKeywords(CardKeyword.Ethereal, MokouModKeywords.Fury, CardKeyword.Exhaust);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.TalonC;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);
        if (FuryActive)
        {
            foreach (var enemy in CombatState.HittableEnemies)
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.75f));
            await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }
}