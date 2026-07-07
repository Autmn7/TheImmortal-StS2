using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class MusterForBattle : MokouModCard
{
    public MusterForBattle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithPower<VigorPower>(4, 2);
        WithTip(typeof(VigorousEnchantment));
        WithTip(new TooltipSource(card => new HoverTip(new LocString("cards", Id.Entry + ".extraTipTitle"), new LocString("cards", Id.Entry + ".extraTipDescription"))));
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target?.Player == null)
            return;
        await PowerCmd.Apply<VigorPower>(choiceContext, cardPlay.Target, DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        foreach (var card in PileType.Hand.GetPile(cardPlay.Target.Player).Cards.ToList())
        {
            var newEnchant = ModelDb.GetById<EnchantmentModel>(enchantment.Id).ToMutable();
            if (newEnchant.CanEnchant(card))
                Enchant(newEnchant, card);
        }
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (cardSource == this && power is VigorPower)
            return target?.Player?.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD" ? 2M : 0M;

        return 0M;
    }
}