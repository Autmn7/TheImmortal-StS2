using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Common;

public class WarmUp : MokouModCard
{
    public WarmUp() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVars(new PowerVar<BurnPower>(2), new PowerVar<VigorPower>(3), new CardsVar(1));
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithTip(typeof(VigorousEnchantment));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue,
            Owner.Creature, this);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
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

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}