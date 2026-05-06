using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class CrimsonWatchguard : MokouModCard
{
    public CrimsonWatchguard() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new DynamicVar("CrimsonWatchguardPower", 1M));
        WithTip(typeof(VigorPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CrimsonWatchguardPower>(this, DynamicVars["CrimsonWatchguardPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}