using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Common;

public class Vitality : MokouModCard
{
    public Vitality() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7);
        WithVars(new PowerVar<VigorPower>(2));
        WithTip(typeof(VigorousEnchantment));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CommonActions.Apply<VigorPower>(Owner.Creature, this, DynamicVars["VigorPower"].IntValue);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var card = (await CardSelectCmd.FromHand(choiceContext, Owner,
                new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
                (Func<CardModel, bool>)(model => enchantment.CanEnchant(model)), this))
            .FirstOrDefault();
        if (card == null)
            return;
        Enchant(enchantment, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        DynamicVars["VigorPower"].UpgradeValueBy(1M);
    }
}