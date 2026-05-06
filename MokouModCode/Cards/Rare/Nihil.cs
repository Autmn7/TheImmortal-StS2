using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Cards.Rare;

public class Nihil : MokouModCard
{
    public Nihil() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Add);
        WithTip(typeof(Exist));
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext context, CardModel card, bool causedByEthereal)
    {
        if (card == this)
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Exist>(Owner),
                    PileType.Discard, Owner));
            await Cmd.Wait(0.5f);
        }
    }
}