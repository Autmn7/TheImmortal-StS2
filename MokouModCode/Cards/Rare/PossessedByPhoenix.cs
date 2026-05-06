using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Cards.Rare;

public class PossessedByPhoenix : MokouModCard
{
    public PossessedByPhoenix() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Feather));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, 99);
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            null, this);
        foreach (var card in cards)
            await CardCmd.Exhaust(choiceContext, card);
        await Feather.CreateInHand(Owner, 10 - Owner.PlayerCombatState.Hand.Cards.Count, CombatState);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}