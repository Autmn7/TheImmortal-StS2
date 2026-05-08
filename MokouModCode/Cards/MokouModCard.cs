using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Character;
using MokouMod.MokouModCode.Extensions;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards;

[Pool(typeof(MokouModCardPool))]
public abstract class MokouModCard : ConstructedCardModel
{
    protected MokouModCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
        WithTip(new TooltipSource((Func<CardModel, IHoverTip>)((CardModel card) =>
            (IHoverTip)(object)new HoverTip(new LocString("static_hover_tips", "MOKOUMOD-ARTIST-TITLE"),
                new LocString("cards", ((AbstractModel)this).Id.Entry + ".artist"), (Texture2D)null))));
    }

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    // public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public virtual Character.MokouMod.Animation Animation => Character.MokouMod.Animation.None;

    public bool IgniteActive { get; private set; }
    public bool FuryActive { get; private set; }
    public bool EmberActive { get; private set; }

    protected decimal CalculateNonLethal(decimal hpLoss)
    {
        if (CombatState == null) return 0M;
        return Owner.Creature.CurrentHp - hpLoss < 1 ? Owner.Creature.CurrentHp - 1 : hpLoss;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;

        if (Keywords.Contains(MokouModKeywords.Ignite))
            IgniteActive = TriggeredIgnite(this, DynamicVars.TryGetValue("Ignite", out var v) ? v.IntValue : 0);
        if (Keywords.Contains(MokouModKeywords.Fury))
            FuryActive = TriggeredFury(this);
        if (Keywords.Contains(MokouModKeywords.Ember))
            EmberActive = TriggeredEmber(this);

        await OnPlayMokou(choiceContext, cardPlay);

        if (IgniteActive || FuryActive || EmberActive)
            foreach (var card in PileType.Hand.GetPile(player).Cards.ToList())
                if (card is MokouModFuelCard fuelCard)
                    await fuelCard.TriggerFuel(player);

        IgniteActive = false;
        FuryActive = false;
        EmberActive = false;
    }

    protected virtual Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }


    public static bool TriggeredIgnite(MokouModCard card, int requiredBurn)
    {
        var player = card.Owner;
        var combat = player.Creature.CombatState;
        if (combat == null || !card.Keywords.Contains(MokouModKeywords.Ignite)) return false;
        var result = combat.Players.Any(p => p.Creature.GetPowerAmount<BurnPower>() >= requiredBurn) ||
                     combat.HittableEnemies.Any(e => e.GetPowerAmount<BurnPower>() >= requiredBurn);
        if (result)
            foreach (var cardInHand in PileType.Hand.GetPile(player).Cards)
                if (cardInHand is MokouModFuelCard fuelCard)
                    fuelCard.Triggered = true;

        return result;
    }

    public static bool TriggeredFury(MokouModCard card)
    {
        var player = card.Owner;
        if (player.Creature.CombatState == null || !card.Keywords.Contains(MokouModKeywords.Fury)) return false;
        var result = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Any(
            (Func<DamageReceivedEntry, bool>)(e =>
                e.HappenedThisTurn(player.Creature.CombatState) && e.Receiver == player.Creature &&
                e.Result.UnblockedDamage > 0));
        if (result)
            foreach (var cardInHand in PileType.Hand.GetPile(player).Cards)
                if (cardInHand is MokouModFuelCard fuelCard)
                    fuelCard.Triggered = true;

        return result;
    }

    public static bool TriggeredEmber(MokouModCard card)
    {
        var player = card.Owner;
        if (player.Creature.CombatState == null || !card.Keywords.Contains(MokouModKeywords.Ember)) return false;
        var state = MokouKeywordStateRegistry.Get(card.Owner);
        var result = player.Creature.CurrentHp <= player.Creature.MaxHp * 0.5f || state.emberTriggeredThisCombat;
        if (result)
            foreach (var cardInHand in PileType.Hand.GetPile(player).Cards)
                if (cardInHand is MokouModFuelCard fuelCard)
                    fuelCard.Triggered = true;

        return result;
    }

    public static EnchantmentModel? Enchant(
        EnchantmentModel enchantment,
        CardModel card,
        decimal amount = 1M)
    {
        enchantment.AssertMutable();
        if (!enchantment.CanEnchant(card))
            throw new InvalidOperationException($"Cannot enchant {card.Id} with {enchantment.Id}.");
        if (card.Enchantment == null)
        {
            card.EnchantInternal(enchantment, amount);
            enchantment.ModifyCard();
        }
        else
        {
            if (!(card.Enchantment.GetType() == enchantment.GetType()))
                throw new InvalidOperationException(
                    $"Cannot enchant {card.Id} with {enchantment.Id} because it already has enchantment {card.Enchantment.Id}.");
            card.Enchantment.Amount += (int)amount;
        }

        card.FinalizeUpgradeInternal();
        return card.Enchantment;
    }
}