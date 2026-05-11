using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Relics;

public class LumpOfCoal : MokouModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("Cinders", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var items = new List<IHoverTip>();
            items.AddRange(HoverTipFactory.FromCard<RedCinder>());
            items.AddRange(HoverTipFactory.FromCard<YellowCinder>());
            items.AddRange(HoverTipFactory.FromCard<BlackCinder>());
            items.Add(HoverTipFactory.FromKeyword(MokouModKeywords.Fuel));
            return items;
        }
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            return;
        Flash();
        IEnumerable<CardModel> cinderCards =
        [
            ModelDb.Card<RedCinder>(),
            ModelDb.Card<YellowCinder>(),
            ModelDb.Card<BlackCinder>()
        ];
        var cinders = CardFactory.GetForCombat(Owner, cinderCards, DynamicVars["Cinders"].IntValue,
            Owner.RunState.Rng.CombatCardGeneration);
        foreach (var cinder in cinders.ToList())
            await CardPileCmd.AddGeneratedCardToCombat(cinder, PileType.Hand, Owner);
    }
}