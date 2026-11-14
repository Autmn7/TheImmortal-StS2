using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Ancient;

public class DevastatingKick : MokouModCard
{
    public DevastatingKick() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithPower<BurnPower>(6, 4);
        WithVars(new HpLossVar(2), new RepeatVar(2));
        WithKeyword(MokouModKeywords.Nonlethal);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackSweepKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nonLethal = CalculateNonLethal(Owner.Creature, DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, nonLethal, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay.Target).WithHitCount(DynamicVars.Repeat.IntValue).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.75f));
        await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || CombatState == null || creature.Side == Owner.Creature.Side)
            return;
        if (Pile != null && Pile.Type != PileType.Hand)
            await CardPileCmd.Add(this, PileType.Hand);
    }
}