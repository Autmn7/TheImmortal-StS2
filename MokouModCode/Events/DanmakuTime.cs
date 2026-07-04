using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Extensions;
using MokouMod.MokouModCode.Scripts;
using static MokouMod.MokouModCode.Cards.MokouModCard;

namespace MokouMod.MokouModCode.Events;

public class DanmakuTime : CustomEventModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HpLossVar(9)];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.CurrentActIndex == 2;
    }

    public override LocString InitialDescription
    {
        get
        {
            if (Owner.Character is Character.MokouMod)
                return L10NLookup("MOKOUMOD-DANMAKU_TIME_MOKOU.pages.INITIAL.description");
            if (Owner.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD")
                return L10NLookup("MOKOUMOD-DANMAKU_TIME_KEINE.pages.INITIAL.description");
            return L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.INITIAL.description");
        }
    }

    public override string CustomInitialPortraitPath => "danmaku_time.png".EventImagePath();

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
        if (Owner.Character is Character.MokouMod)
            return new List<EventOption>
            {
                CreateManualOption(Talk, "MOKOUMOD-DANMAKU_TIME_MOKOU.pages.INITIAL.options.TALK"),
                CreateManualOption(Fight, "MOKOUMOD-DANMAKU_TIME_MOKOU.pages.INITIAL.options.FIGHT", [HoverTipFactory.FromKeyword(MokouModKeywords.Nonlethal), ..HoverTipFactory.FromCardWithCardHoverTips<DualSuppression>()])
            };
        if (Owner.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD")
            return new List<EventOption>
            {
                CreateManualOption(Talk, "MOKOUMOD-DANMAKU_TIME_KEINE.pages.INITIAL.options.TALK"),
                CreateManualOption(Fight, "MOKOUMOD-DANMAKU_TIME_KEINE.pages.INITIAL.options.FIGHT", [HoverTipFactory.FromKeyword(MokouModKeywords.Nonlethal), ..HoverTipFactory.FromCardWithCardHoverTips<DualSuppression>()])
            };
        return new List<EventOption>
        {
            CreateManualOption(Talk, "MOKOUMOD-DANMAKU_TIME.pages.INITIAL.options.TALK"),
            CreateManualOption(Fight, "MOKOUMOD-DANMAKU_TIME.pages.INITIAL.options.FIGHT", [HoverTipFactory.FromKeyword(MokouModKeywords.Nonlethal), ..HoverTipFactory.FromCardWithCardHoverTips<DualSuppression>()])
        };
    }

    private async Task Talk()
    {
        var card = (await CardSelectCmd.FromDeckForUpgrade(Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1))).FirstOrDefault();
        if (card != null)
            CardCmd.Upgrade(card);
        if (Owner.Character is Character.MokouMod)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME_MOKOU.pages.TALK.description"));
        else if (Owner.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD")
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME_KEINE.pages.TALK.description"));
        else
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.TALK.description"));
    }

    private async Task Fight()
    {
        var nonLethal = CalculateNonLethal(Owner.Creature, DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, nonLethal, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        var card = Owner.RunState.CreateCard<DualSuppression>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        if (Owner.Character is Character.MokouMod)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME_MOKOU.pages.FIGHT.description"));
        else if (Owner.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD")
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME_KEINE.pages.FIGHT.description"));
        else if (Owner.Character is Ironclad)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT_IRONCLAD.description"));
        else if (Owner.Character is Silent)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT_SILENT.description"));
        else if (Owner.Character is Regent)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT_REGENT.description"));
        else if (Owner.Character is Necrobinder)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT_NECROBINDER.description"));
        else if (Owner.Character is Defect)
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT_DEFECT.description"));
        else
            SetEventFinished(L10NLookup("MOKOUMOD-DANMAKU_TIME.pages.FIGHT.description"));
    }
}