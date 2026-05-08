using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Relics;
using MokouMod.MokouModCode.Scripts;
using static MokouMod.MokouModCode.Character.MokouMod;

namespace MokouMod.MokouModCode.Powers;

public class RekindlePower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("HealPercent")];

    private decimal GetHealPercent()
    {
        var player = Owner.Player;
        if (player == null)
            return 15M;
        var essence = 0;
        var tempEssence = 0;
        if (player.GetRelic<ImmortalPlume>() != null)
            essence = player.GetRelic<ImmortalPlume>()!.Essence;
        else if (player.GetRelic<BurningPlume>() != null) essence = player.GetRelic<BurningPlume>()!.Essence;

        if (Owner.HasPower<TempEssencePower>()) tempEssence = Owner.GetPowerAmount<TempEssencePower>();

        return 15M + (essence + tempEssence) * 5M;
    }

    private void UpdateHealPercentVar()
    {
        var value = (int)GetHealPercent();
        var dynamicVar = (StringVar)DynamicVars["HealPercent"];
        dynamicVar.StringValue = value.ToString();
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is RekindlePower or TempEssencePower) UpdateHealPercentVar();

        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override bool ShouldDie(Creature creature)
    {
        return creature != Owner;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature.Player == null || creature != Owner)
            return;
        MokouKeywordStateRegistry.Get(creature.Player).emberTriggeredThisCombat = true;

        RunAnimation(creature.Player, Animation.Resurrection);

        var healPercent = GetHealPercent();
        var healAmount = Math.Max(1M, creature.MaxHp * (healPercent / 100M));
        await CreatureCmd.Heal(creature, healAmount);
        if (creature.HasPower<BurnPower>() && !creature.HasPower<PhoenixFormPower>())
            await PowerCmd.Remove(creature.GetPower<BurnPower>()!);

        if (creature.HasPower<LegendOfImmortalityPower>())
            await creature.GetPower<LegendOfImmortalityPower>()!.OnTrigger();

        await PowerCmd.Decrement(this);

        if (creature.Player != null)
            foreach (var card in PileType.Exhaust.GetPile(creature.Player).Cards.ToList())
            {
                if (card.Id.Entry.Equals("MOKOUMOD-IMPERISHABLE_SHOOTING"))
                {
                    card.DynamicVars.Damage.BaseValue *= 2;
                    card.DynamicVars["BurnPower"].BaseValue *= 2;
                    await CardPileCmd.Add(card, PileType.Hand);
                }

                if (card.Id.Entry.Equals("MOKOUMOD-PHOENIX_REBIRTH")) await CardPileCmd.Add(card, PileType.Hand);
            }
    }
}