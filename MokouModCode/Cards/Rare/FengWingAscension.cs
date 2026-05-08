using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class FengWingAscension : MokouModCard
{
    public FengWingAscension() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVars(new RepeatVar(3), new IgniteVar(10M));
        WithTip(typeof(BurnPower));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackAirKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue).BeforeDamage(async () =>
            {
                var creatureNode = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
                if (creatureNode != null)
                {
                    var child = NLargeMagicMissileVfx.Create(creatureNode.GetBottomOfHitbox(), new Color("#c81e1e"));
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
                }

                await Cmd.CustomScaledWait(0.3f, 0.4f);
            }).Execute(choiceContext);
        var burn = attackCommand.Results.Sum(results => results.Sum(r => r.UnblockedDamage));
        if (burn > 0)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely((Node)NGroundFireVfx.Create(cardPlay.Target));
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely((Node)NFireBurstVfx.Create(cardPlay.Target, 0.75f));
            await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, (decimal)(0.5f * burn), Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1M);
    }
}