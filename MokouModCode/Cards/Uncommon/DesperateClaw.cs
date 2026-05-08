using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class DesperateClaw : MokouModCard
{
    public DesperateClaw() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVars(
            new CalculationBaseVar(1M),
            new CalculationExtraVar(1M),
            new CalculatedVar("CalculatedHits")
                .WithMultiplier((Func<CardModel, Creature?, decimal>)((card, _) =>
                {
                    var total = 0;
                    var power = card.Owner.Creature.GetPower<SharpenedPower>();
                    if (power != null)
                        total += power.Amount;
                    return total;
                }))
        );
        WithKeywords(MokouModKeywords.Fury, MokouModKeywords.Ember);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
            .WithHitVfxNode((Func<Creature, Node2D>)(t => NScratchVfx.Create(t, true))).Execute(choiceContext);
        if (FuryActive || EmberActive) await PowerCmd.Apply<SharpenedPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}