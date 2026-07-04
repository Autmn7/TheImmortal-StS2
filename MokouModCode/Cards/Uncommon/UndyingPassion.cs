using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
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
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "slash_attack.mp3").Execute(choiceContext);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        if (enchantment.CanEnchant(this))
            Enchant(enchantment, this);
        var enchantment2 = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards =
            (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                PileType.Hand.GetPile(Owner).Cards.Concat(PileType.Draw.GetPile(Owner).Cards.OrderBy(c => c.Rarity).ThenBy(c => c.Id))
                    .Where(model => enchantment.CanEnchant(model) && model.Type == CardType.Attack)
                    .ToList(),
                Owner,
                new CardSelectorPrefs(new LocString("card_selection", "TO_ENCHANT_VIGOROUS"), DynamicVars.Cards.IntValue)
            )).ToList();
        if (cards.Count != 0)
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