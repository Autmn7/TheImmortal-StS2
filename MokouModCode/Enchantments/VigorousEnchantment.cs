using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Extensions;

namespace MokouMod.MokouModCode.Enchantments;

public class VigorousEnchantment : CustomEnchantmentModel
{
    public override bool ShowAmount => false;

    public override bool HasExtraCardText => true;

    protected override string? CustomIconPath => "vigorous_enchantment.png".CharacterUiPath();

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay.Card != Card || cardPlay.Resources.EnergySpent <= 0)
            return;
        if (Card.Type != CardType.Attack)
            await PowerCmd.Apply<VigorPower>(choiceContext, cardPlay.Card.Owner.Creature,
                2 * cardPlay.Resources.EnergySpent, cardPlay.Card.Owner.Creature, Card);
    }
}