using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class FiveColoredTurtle : MokouModCard
{
    public FiveColoredTurtle() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("FiveColoredTurtlePower", 2M));
        WithTip(typeof(VigorPower));
        WithTip(typeof(VigorousEnchantment));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FiveColoredTurtlePower>(choiceContext, Owner.Creature, DynamicVars["FiveColoredTurtlePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FiveColoredTurtlePower"].UpgradeValueBy(1M);
    }
}