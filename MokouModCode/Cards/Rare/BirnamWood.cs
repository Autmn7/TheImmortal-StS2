using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class BirnamWood : MokouModFuelCard
{
    public BirnamWood() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        Durability = MaxDurability = 3M;
        WithVars(new DurabilityVar(3));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<TragicFate>(card.IsUpgraded)));
    }

    protected override async Task OnFuelDurabilityDeplete()
    {
        var card = CombatState.CreateCard<TragicFate>(Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}