using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class WoundsOfMetsuzai : MokouModCard
{
    public WoundsOfMetsuzai() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithVars(new DynamicVar("Ratio", 5M));
        WithKeyword(CardKeyword.Exhaust);
        WithTip(MokouModKeywords.Exhume);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var unblockedDamage = attackCommand.Results.Sum(results => results.Sum(r => r.UnblockedDamage));
        var cardNum = (int)Math.Floor(unblockedDamage / DynamicVars["Ratio"].BaseValue);
        var cards =
            await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                PileType.Exhaust.GetPile(Owner).Cards
                    .ToList(),
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, cardNum)
            );
        foreach (var card in cards)
            await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}