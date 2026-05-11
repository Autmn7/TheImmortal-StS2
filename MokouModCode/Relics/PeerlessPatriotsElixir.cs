using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MokouMod.MokouModCode.Enchantments;
using static MokouMod.MokouModCode.Cards.MokouModCard;

namespace MokouMod.MokouModCode.Relics;

public class PeerlessPatriotsElixir : MokouModRelic
{
    private bool _triggeredThisTurn = false;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [..HoverTipFactory.FromEnchantment<VigorousEnchantment>()];

    private bool TriggeredThisTurn
    {
        get => _triggeredThisTurn;
        set
        {
            AssertMutable();
            _triggeredThisTurn = value;
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (!TriggeredThisTurn && card.Type == CardType.Attack && card.Enchantment == null )
        {
            Flash();
            var enchantment = ModelDb.Enchantment<VigorousEnchantment>().ToMutable();
            if (enchantment.CanEnchant(card))
                Enchant(enchantment, card);
            TriggeredThisTurn = true;
            Status = RelicStatus.Normal;
        }
        return Task.CompletedTask;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
            return Task.CompletedTask;
        TriggeredThisTurn = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        TriggeredThisTurn = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}