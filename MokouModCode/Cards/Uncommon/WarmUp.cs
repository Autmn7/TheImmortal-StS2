using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class WarmUp : MokouModCard
{
    public WarmUp() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<VigorPower>(3, 1);
        WithVar(new IgniteVar(5));
        WithEnergy(1, 1);
        WithKeywords(MokouModKeywords.Ignite, MokouModKeywords.Fury);
        WithTip(typeof(VigorousEnchantment));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var xValue = ResolveEnergyXValue();
        var vigorAmt = xValue * DynamicVars["VigorPower"].BaseValue;

        if (vigorAmt > 0)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, vigorAmt, Owner.Creature, this);
        }

        if (xValue > 0)
        {
            var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
            var cards = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(new LocString("card_selection", "TO_ENCHANT_VIGOROUS"), xValue), (Func<CardModel, bool>)(model => enchantment.CanEnchant(model)), this)).ToList();
            if (cards.Count != 0)
                foreach (var card in cards)
                {
                    var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
                    Enchant(newEnchant, card);
                }
        }

        if (IgniteActive || FuryActive)
        {
            var energy = DynamicVars.Energy.BaseValue > cardPlay.Resources.EnergySpent ? cardPlay.Resources.EnergySpent : DynamicVars.Energy.BaseValue;
            if (energy > 0)
                await PlayerCmd.GainEnergy(energy, Owner);
        }
    }
}