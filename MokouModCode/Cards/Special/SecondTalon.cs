using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class SecondTalon : MokouModCard
{
    public SecondTalon() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<FinalTalon>(card.IsUpgraded)));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitVfxNode((Func<Creature, Node2D>)(t => NScratchVfx.Create(t, true))).Execute(choiceContext);
        var card = CombatState.CreateCard<FinalTalon>(Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(card);
        if (Enchantment != null)
        {
            var enchantment = (EnchantmentModel)Enchantment.MutableClone();
            Enchant(enchantment, card, Enchantment.Amount);
        }

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}