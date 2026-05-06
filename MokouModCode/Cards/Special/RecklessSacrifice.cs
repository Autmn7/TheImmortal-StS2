using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class RecklessSacrifice : MokouModCard
{
    public RecklessSacrifice() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithVars(
            new CalculationBaseVar(0M),
            new ExtraDamageVar(1M),
            new CalculatedDamageVar(ValueProp.Move)
                .WithMultiplier((Func<CardModel, Creature?, decimal>)((card, _) =>
                {
                    return card.Owner.Creature.MaxHp - card.Owner.Creature.CurrentHp;
                }))
        );
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}