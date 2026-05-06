using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class BlazingStorm : MokouModCard
{
    public BlazingStorm() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithVars(new PowerVar<BurnPower>(1), new RepeatVar(3), new IgniteVar(8M));
        WithKeywords(MokouModKeywords.Ignite);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitCount(IgniteActive ? DynamicVars.Repeat.IntValue + 1 : DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        await Cmd.CustomScaledWait(0.2f, 0.3f);
        for (var i = 0; i < (IgniteActive ? DynamicVars.Repeat.IntValue + 1 : DynamicVars.Repeat.IntValue); ++i)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.5f));
            await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, DynamicVars["BurnPower"].BaseValue,
                Owner.Creature, this);
            await Cmd.CustomScaledWait(0.1f, 0.2f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }
}