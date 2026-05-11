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
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class WarmUp : MokouModCard
{
    public WarmUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<BurnPower>(1), new PowerVar<VigorPower>(2), new CardsVar(1), new IgniteVar(5), new EnergyVar(1));
        WithKeywords(MokouModKeywords.Ignite, MokouModKeywords.Fury);
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
        if (FuryActive)
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(1M);
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}