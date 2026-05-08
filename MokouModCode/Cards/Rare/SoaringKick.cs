using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Rare;

public class SoaringKick : MokouModCard
{
    public SoaringKick() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(12);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(typeof(VigorousEnchantment));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards = PileType.Hand.GetPile(Owner).Cards.Concat(PileType.Draw.GetPile(Owner).Cards)
            .Concat(PileType.Discard.GetPile(Owner).Cards).Concat(PileType.Exhaust.GetPile(Owner).Cards);
        if (!cards.Any())
            return;
        foreach (var card in cards)
            if (card.Type == CardType.Attack && enchantment.CanEnchant(card))
            {
                var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
                Enchant(newEnchant, card);
            }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}