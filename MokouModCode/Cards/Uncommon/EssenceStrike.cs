using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class EssenceStrike : MokouModCard
{
    public EssenceStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVars(
            new CalculationBaseVar(1M),
            new CalculationExtraVar(1M),
            new CalculatedVar("CalculatedHits")
                .WithMultiplier((Func<CardModel, Creature?, decimal>)((card, _) =>
                {
                    var total = 0;
                    var player = card.Owner;
                    var immortal = player.GetRelic<ImmortalPlume>();
                    var burning = player.GetRelic<BurningPlume>();
                    if (immortal != null)
                        total += immortal.Essence;
                    else if (burning != null) total += burning.Essence;

                    var power = player.Creature.GetPower<TempEssencePower>();
                    if (power != null)
                        total += power.Amount;
                    return total;
                }))
        );
        WithTags(CardTag.Strike);
        WithTip(MokouModKeywords.Essence);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
            .FromCard(this).BeforeDamage(async () =>
            {
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.5f));
            })
            .WithHitFx("vfx/vfx_attack_slash").Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}