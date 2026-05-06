using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class FanTheFlames : MokouModCard
{
    public FanTheFlames() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("FanTheFlamesPower", 3M));
        WithTip(MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FanTheFlamesPower>(this, DynamicVars["FanTheFlamesPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FanTheFlamesPower"].UpgradeValueBy(2M);
    }
}