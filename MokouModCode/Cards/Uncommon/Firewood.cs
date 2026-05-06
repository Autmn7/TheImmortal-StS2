using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Firewood : MokouModFuelCard
{
    public Firewood() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        Durability = MaxDurability = 2M;
        WithVars(new DurabilityVar(2));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(typeof(RedCinder));
        WithTip(typeof(YellowCinder));
        WithTip(typeof(BlackCinder));
    }

    protected override async Task OnFuelTrigger()
    {
        IEnumerable<CardModel> cinderCards =
        [
            ModelDb.Card<RedCinder>(),
            ModelDb.Card<YellowCinder>(),
            ModelDb.Card<BlackCinder>()
        ];
        var cinder = CardFactory
            .GetDistinctForCombat(Owner, cinderCards, 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        await CardPileCmd.AddGeneratedCardToCombat(cinder, PileType.Hand, Owner);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext context, CardModel card, bool causedByEthereal)
    {
        if (card == this)
        {
            IEnumerable<CardModel> cinderCards =
            [
                ModelDb.Card<RedCinder>(),
                ModelDb.Card<YellowCinder>(),
                ModelDb.Card<BlackCinder>()
            ];
            var cinder = CardFactory
                .GetDistinctForCombat(Owner, cinderCards, 1, Owner.RunState.Rng.CombatCardGeneration)
                .FirstOrDefault();
            await CardPileCmd.AddGeneratedCardToCombat(cinder, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Durability"].UpgradeValueBy(1M);
        Durability += 1M;
        MaxDurability += 1M;
    }
}