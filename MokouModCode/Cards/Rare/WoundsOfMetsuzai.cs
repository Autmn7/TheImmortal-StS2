using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellCast;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        var unblockedDamage = attackCommand.Results.Sum(results => results.Sum(r => r.TotalDamage + r.OverkillDamage));
        var cardNum = (int)Math.Floor(unblockedDamage / DynamicVars["Ratio"].BaseValue);
        if (cardNum > 0)
        {
            var cards = await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Exhaust.GetPile(Owner).Cards.ToList(), Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, cardNum));
            foreach (var card in cards)
                await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}