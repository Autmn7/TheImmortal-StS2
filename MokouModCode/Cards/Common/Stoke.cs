using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class Stoke : MokouModCard
{
    public Stoke() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithVars(new CardsVar("Cinders", 1));
        WithTip(typeof(RedCinder));
        WithTip(typeof(YellowCinder));
        WithTip(typeof(BlackCinder));
        WithTip(MokouModKeywords.Fuel);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cinderCards =
        [
            ModelDb.Card<RedCinder>(),
            ModelDb.Card<YellowCinder>(),
            ModelDb.Card<BlackCinder>()
        ];
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .Execute(choiceContext);
        var cinders = CardFactory.GetForCombat(Owner, cinderCards, DynamicVars["Cinders"].IntValue,
            Owner.RunState.Rng.CombatCardGeneration);
        foreach (var cinder in cinders.ToList())
            await CardPileCmd.AddGeneratedCardToCombat(cinder, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cinders"].UpgradeValueBy(1M);
    }
}