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
public class RedCinder : MokouModFuelCard
{
    public RedCinder() : base(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        Durability = MaxDurability = 2M;
        WithVars(new DurabilityVar(2), new PowerVar<BurnPower>(4));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card == this)
            await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies, DynamicVars["BurnPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BurnPower"].UpgradeValueBy(2M);
    }
}