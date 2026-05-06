using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class SpontaneousCombustion : MokouModCard
{
    public SpontaneousCombustion() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("SpontaneousCombustionPower", 1M));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpontaneousCombustionPower>(this,
            DynamicVars["SpontaneousCombustionPower"].IntValue);
    }
}