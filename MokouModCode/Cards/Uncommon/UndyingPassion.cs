using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class UndyingPassion : MokouModCard
{
    public UndyingPassion() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithTip(typeof(VigorousEnchantment));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "slash_attack.mp3").Execute(choiceContext);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        if (enchantment.CanEnchant(this))
            Enchant(enchantment, this);
        var enchantment2 = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var card = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
            (Func<CardModel, bool>)(model => enchantment.CanEnchant(model) && model.Type == CardType.Attack),
            this)).FirstOrDefault();
        if (card == null)
            return;
        Enchant(enchantment2, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}