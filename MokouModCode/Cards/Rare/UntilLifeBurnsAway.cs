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

namespace MokouMod.MokouModCode.Cards.Rare;

public class UntilLifeBurnsAway : MokouModCard
{
    public UntilLifeBurnsAway() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithVars(
            new DynamicVar("Ratio", 6M),
            new CalculationBaseVar(0M),
            new CalculationExtraVar(1M),
            new CalculatedVar("CalculatedHpLoss")
                .WithMultiplier((Func<CardModel, Creature?, decimal>)((card, _) =>
                {
                    var currentHp = card.Owner.Creature.CurrentHp;
                    return currentHp > 1 ? (decimal)currentHp - 1 : 0M;
                })),
            new CalculatedVar("CalculatedEnergy")
                .WithMultiplier((card, _) =>
                {
                    var currentHp = card.Owner.Creature.CurrentHp;
                    var hpLoss = currentHp > 1 ? (decimal)currentHp - 1 : 0M;
                    var ratio = card.DynamicVars["Ratio"].BaseValue;
                    return Math.Floor(hpLoss / ratio);
                })
        );
        WithKeyword(CardKeyword.Exhaust);
        WithTip(typeof(BurnPower));
        WithEnergyTip();
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hpLoss = ((CalculatedVar)DynamicVars["CalculatedHpLoss"]).Calculate(cardPlay.Target);
        var energy = ((CalculatedVar)DynamicVars["CalculatedEnergy"]).Calculate(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        if (energy > 0)
            await PlayerCmd.GainEnergy(energy, Owner);
        if (hpLoss > 0)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(enemy));
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(enemy, 0.75f));
            }

            await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies, hpLoss, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Ratio"].UpgradeValueBy(-1M);
    }
}