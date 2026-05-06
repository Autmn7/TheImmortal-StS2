using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class BuildUp : MokouModCard
{
    public BuildUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<VigorPower>(4));
        WithKeyword(MokouModKeywords.Fury);
        WithTip(typeof(VigorousEnchantment));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue,
            Owner.Creature, this);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, FuryActive ? 2 : 1),
            (Func<CardModel, bool>)(model => enchantment.CanEnchant(model)), this);
        if (!cards.Any())
            return;
        foreach (var card in cards)
        {
            var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
            Enchant(newEnchant, card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(2M);
    }
}