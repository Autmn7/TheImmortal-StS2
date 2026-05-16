using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Powers;

public class EternalFlamePower : MokouModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (Owner.Player == null || player != Owner.Player)
            return;
        Flash();
        IEnumerable<CardModel> cinderCards =
        [
            ModelDb.Card<RedCinder>(),
            ModelDb.Card<YellowCinder>(),
            ModelDb.Card<BlackCinder>()
        ];
        var cinders = CardFactory.GetDistinctForCombat(Owner.Player, cinderCards, Amount,
            Owner.Player.RunState.Rng.CombatCardGeneration);
        foreach (var cinder in cinders.ToList().Cast<MokouModFuelCard?>())
        {
            cinder!.Durability--;
            await CardPileCmd.AddGeneratedCardToCombat(cinder, PileType.Hand, Owner.Player);
        }
    }
}