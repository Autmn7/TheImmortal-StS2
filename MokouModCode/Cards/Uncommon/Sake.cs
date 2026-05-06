using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Sake : MokouModFuelCard
{
    public Sake() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        Durability = MaxDurability = 3M;
        WithVars(new DurabilityVar(3), new PowerVar<VigorPower>(1));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnFuelTrigger()
    {
        await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card == this)
            if (Owner.Creature.HasPower<VigorPower>())
                await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature,
                    Owner.Creature.GetPowerAmount<VigorPower>(), Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(1M);
    }
}