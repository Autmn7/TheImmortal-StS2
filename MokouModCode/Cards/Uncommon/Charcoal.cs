using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Charcoal : MokouModFuelCard
{
    public Charcoal() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        Durability = MaxDurability = 2M;
        WithVars(new DurabilityVar(2), new PowerVar<BurnPower>(2), new CardsVar(1));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnFuelTrigger()
    {
        var context = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId ?? 0,
            GameActionType.Combat
        );
        await CardPileCmd.Draw(context, 1, Owner);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card == this)
        {
            await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue,
                Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}