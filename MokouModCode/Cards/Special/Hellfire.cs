using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class Hellfire : MokouModFuelCard
{
    public Hellfire() : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
    {
        Durability = MaxDurability = 4M;
        WithVars(new DurabilityVar(4), new PowerVar<HellfirePower>(4));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
    }

    public override int MaxUpgradeLevel => 0;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this) 
            await PowerCmd.Apply<HellfirePower>(choiceContext, CombatState.HittableEnemies, DynamicVars["HellfirePower"].IntValue, Owner.Creature, this);
    }
}