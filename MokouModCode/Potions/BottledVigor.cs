using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;
using static MokouMod.MokouModCode.Cards.MokouModCard;

namespace MokouMod.MokouModCode.Potions;

public class BottledVigor : MokouModPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>(5), new CardsVar(2)];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VigorPower>(), ..HoverTipFactory.FromEnchantment<VigorousEnchantment>()];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, target, DynamicVars["VigorPower"].BaseValue, Owner.Creature, null);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards = await CardSelectCmd.FromHand(choiceContext, target.Player,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, DynamicVars.Cards.IntValue),
            (Func<CardModel, bool>)(model => enchantment.CanEnchant(model)), this);
        if (!cards.Any())
            return;
        foreach (var card in cards)
        {
            var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
            Enchant(newEnchant, card);
        }
    }
}