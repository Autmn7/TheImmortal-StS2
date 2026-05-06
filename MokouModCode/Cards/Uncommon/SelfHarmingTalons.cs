using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class SelfHarmingTalons : MokouModCard
{
    public SelfHarmingTalons() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4);
        WithVars(new HpLossVar(2));
        WithKeyword(CardKeyword.Retain);
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<SecondTalon>(card.IsUpgraded)));
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<FinalTalon>(card.IsUpgraded)));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitVfxNode((Func<Creature, Node2D>)(t => NScratchVfx.Create(t, true))).Execute(choiceContext);
        var card = CombatState.CreateCard<SecondTalon>(Owner);
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