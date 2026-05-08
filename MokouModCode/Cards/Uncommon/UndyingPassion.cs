using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class UndyingPassion : MokouModCard
{
    public UndyingPassion() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithVars(new CardsVar(1));
        WithTip(typeof(VigorousEnchantment));
        WithKeyword(CardKeyword.Innate);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackUpKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "slash_attack.mp3").Execute(choiceContext);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        if (enchantment.CanEnchant(this))
            Enchant(enchantment, this);
        var enchantment2 = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, DynamicVars.Cards.IntValue),
            (Func<CardModel, bool>)(model => enchantment.CanEnchant(model) && model.Type == CardType.Attack), this);
        if (!cards.Any())
            return;
        foreach (var card in cards)
        {
            var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment2.Id).ToMutable();
            Enchant(newEnchant, card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
        DynamicVars["Cards"].UpgradeValueBy(1M);
    }
}