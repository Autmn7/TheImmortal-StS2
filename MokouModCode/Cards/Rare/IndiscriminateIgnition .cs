using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class IndiscriminateIgnition : MokouModCard
{
    public IndiscriminateIgnition() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithVars(new DynamicVar("Ratio", 1M));
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<BurnPower>())
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
            await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["Ratio"].IntValue * Owner.Creature.GetPowerAmount<BurnPower>(),
                Owner.Creature, this);
        }

        foreach (var enemy in CombatState.HittableEnemies)
            if (enemy.HasPower<BurnPower>())
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely((Node)NGroundFireVfx.Create(enemy));
                await PowerCmd.Apply<BurnPower>(choiceContext, enemy, DynamicVars["Ratio"].IntValue * enemy.GetPowerAmount<BurnPower>(),
                    Owner.Creature, this);
            }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Ratio"].UpgradeValueBy(1M);
    }
}