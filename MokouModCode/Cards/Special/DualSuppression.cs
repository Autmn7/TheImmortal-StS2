using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class DualSuppression : MokouModCard
{
    public DualSuppression() : base(0, CardType.Skill, CardRarity.Event, TargetType.AllEnemies)
    {
        WithKeywords(CardKeyword.Innate, CardKeyword.Exhaust);
        WithPower<BurnPower>(10);
        WithPower<DupHistoricalGapPower>(6);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> options =
        [
            Owner.Creature.CombatState.CreateCard<MokousSuppression>(Owner),
            Owner.Creature.CombatState.CreateCard<KeinesSuppression>(Owner)
        ];
        if (IsUpgraded)
        {
            foreach (var card in options) await ((IChoosable)card).OnChosen();
        }
        else
        {
            var chosenCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
            await ((IChoosable)chosenCard).OnChosen();
        }
    }

    public interface IChoosable
    {
        Task OnChosen();
    }
}