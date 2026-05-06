using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;
using static MokouMod.MokouModCode.Cards.MokouModCard;

namespace MokouMod.MokouModCode.Powers;

public class FiveColoredTurtlePower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner, Amount, Owner, null);
        var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
        var card = (await CardSelectCmd.FromHand(choiceContext, Owner.Player,
                new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
                (Func<CardModel, bool>)(model => enchantment.CanEnchant(model)), this))
            .FirstOrDefault();
        if (card == null)
            return;
        Enchant(enchantment, card);
    }
}