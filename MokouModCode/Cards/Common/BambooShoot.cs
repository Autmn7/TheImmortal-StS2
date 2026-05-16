using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class BambooShoot : MokouModFuelCard
{
    public BambooShoot() : base(-1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        Durability = MaxDurability = 4M;
        WithBlock(3);
        WithVars(new DurabilityVar(4), new PowerVar<RegenPower>(1));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnFuelTrigger()
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card == this)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, DynamicVars["RegenPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1M);
    }
}