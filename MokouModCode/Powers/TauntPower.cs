using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Cards.Uncommon;

namespace MokouMod.MokouModCode.Powers;

public class TauntPower : MokouModPower
{
    public bool _shouldIgnoreNextInstance;

    public override PowerType Type => !IsPositive ? PowerType.Debuff : PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public AbstractModel OriginModel => ModelDb.Card<TauntMokou>();

    public PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();

    protected virtual bool IsPositive => true;

    public int Sign => !IsPositive ? -1 : 1;

    public override LocString Title
    {
        get
        {
            switch (OriginModel)
            {
                case CardModel cardModel:
                    return cardModel.TitleLocString;
                case PotionModel potionModel:
                    return potionModel.Title;
                case RelicModel relicModel:
                    return relicModel.Title;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public override LocString Description =>
        new("powers",
            IsPositive ? "TEMPORARY_STRENGTH_POWER.description" : "TEMPORARY_STRENGTH_DOWN.description");

    protected override string SmartDescriptionLocKey =>
        !IsPositive
            ? "TEMPORARY_STRENGTH_DOWN.smartDescription"
            : "TEMPORARY_STRENGTH_POWER.smartDescription";

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var items = new List<IHoverTip>();
            var originTips = OriginModel switch
            {
                CardModel card => [HoverTipFactory.FromCard(card)],
                PotionModel potion => [HoverTipFactory.FromPotion(potion)],
                RelicModel relic => HoverTipFactory.FromRelic(relic),
                _ => throw new InvalidOperationException("Unknown OriginModel type")
            };
            items.AddRange(originTips);
            items.Add(HoverTipFactory.FromPower<StrengthPower>());
            return items;
        }
    }

    public void IgnoreNextInstance()
    {
        _shouldIgnoreNextInstance = true;
    }

    public override async Task BeforeApplied(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            var strengthPower = await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(), target, Sign * amount, applier,
                cardSource, true);
        }
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount == Amount || power != this)
            return;
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            var strengthPower = await PowerCmd.Apply<StrengthPower>(choiceContext, Owner,
                Sign * amount, applier, cardSource, true);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        Flash();
        await PowerCmd.Remove(this);
        var strengthPower = await PowerCmd.Apply<StrengthPower>(choiceContext, Owner,
            -Sign * Amount, Owner, null);
    }
}