using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MokouMod.MokouModCode.Extensions;
using MokouMod.MokouModCode.Potions;

namespace MokouMod.MokouModCode.Events;

public class TheMedicineSeller : CustomEventModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("BuyCaseCost", 50), new("PotionSlots", 1)];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Act is Overgrowth && runState.Players.All(p => p.Gold >= DynamicVars["BuyCaseCost"].BaseValue);
    }

    public override LocString InitialDescription
    {
        get
        {
            if (Owner.Character is Character.MokouMod)
                return L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER_MOKOU.pages.INITIAL.description");
            return L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER.pages.INITIAL.description");
        }
    }

    public override string CustomInitialPortraitPath => "the_medicine_seller.png".EventImagePath();

    private EventOption CreateManualOption(Func<Task> onChosen, string optionKey)
    {
        var title = new LocString(LocTable, optionKey + ".title");
        var description = new LocString(LocTable, optionKey + ".description");
        return new EventOption(this, onChosen, title, description, optionKey, Enumerable.Empty<IHoverTip>());
    }

    private EventOption CreateManualOption(Func<Task> onChosen, string optionKey, IEnumerable<IHoverTip> hoverTips)
    {
        var title = new LocString(LocTable, optionKey + ".title");
        var description = new LocString(LocTable, optionKey + ".description");
        return new EventOption(this, onChosen, title, description, optionKey, hoverTips);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var gold = Owner.Gold;
        if (Owner.Character is Character.MokouMod)
            return new List<EventOption>
            {
                CreateManualOption(AcceptElixir, "MOKOUMOD-THE_MEDICINE_SELLER_MOKOU.pages.INITIAL.options.ACCEPT_ELIXIR", [HoverTipFactory.FromPotion<HouraiElixir>()]),
                gold < DynamicVars["BuyCaseCost"].IntValue ? CreateLockedOption() : CreateManualOption(BuyCase, "MOKOUMOD-THE_MEDICINE_SELLER_MOKOU.pages.INITIAL.options.BUY_CASE")
            };
        return new List<EventOption>
        {
            CreateManualOption(AcceptElixir, "MOKOUMOD-THE_MEDICINE_SELLER.pages.INITIAL.options.ACCEPT_ELIXIR"),
            gold < DynamicVars["BuyCaseCost"].IntValue ? CreateLockedOption() : CreateManualOption(BuyCase, "MOKOUMOD-THE_MEDICINE_SELLER.pages.INITIAL.options.BUY_CASE")
        };
    }

    private async Task AcceptElixir()
    {
        if (Owner.Character is Character.MokouMod)
        {
            await RewardsCmd.OfferCustom(Owner, new List<Reward>(1)
            {
                new PotionReward(ModelDb.Potion<HouraiElixir>().ToMutable(), Owner)
            });
            SetEventFinished(L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER_MOKOU.pages.ACCEPT_ELIXIR.description"));
        }
        else
        {
            var items = Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState)).Where((Func<PotionModel, bool>)(p => p.Rarity == PotionRarity.Rare));
            var potionModel = Owner.PlayerRng.Rewards.NextItem(items);
            if (potionModel != null)
                await RewardsCmd.OfferCustom(Owner, new List<Reward>(1)
                {
                    new PotionReward(potionModel.ToMutable(), Owner)
                });
            SetEventFinished(L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER.pages.ACCEPT_ELIXIR.description"));
        }
    }

    private async Task BuyCase()
    {
        await PlayerCmd.LoseGold(DynamicVars["BuyCaseCost"].IntValue, Owner, GoldLossType.Spent);
        await PlayerCmd.GainMaxPotionCount(DynamicVars["PotionSlots"].IntValue, Owner);
        if (Owner.Character is Character.MokouMod)
            SetEventFinished(L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER_MOKOU.pages.BUY_CASE.description"));
        else
            SetEventFinished(L10NLookup("MOKOUMOD-THE_MEDICINE_SELLER.pages.BUY_CASE.description"));
    }

    private EventOption CreateLockedOption()
    {
        return new EventOption(this, null, "MOKOUMOD-THE_MEDICINE_SELLER.pages.INITIAL.options.LOCKED");
    }
}