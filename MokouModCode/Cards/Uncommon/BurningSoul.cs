using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class BurningSoul : MokouModCard
{
    public BurningSoul() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("BurningSoulPower", 1M));
        WithTip(typeof(Feather));
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BurningSoulPower>(this, DynamicVars["BurningSoulPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BurningSoulPower"].UpgradeValueBy(1M);
    }
}