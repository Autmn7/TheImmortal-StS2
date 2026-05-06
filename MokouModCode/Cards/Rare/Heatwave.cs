using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class Heatwave : MokouModCard
{
    public Heatwave() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new CardsVar("Feathers", 2));
        WithTip(typeof(Feather));
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<HeatwavePower>(this, 1);
        await Feather.CreateInHand(Owner, DynamicVars["Feathers"].IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Feathers"].UpgradeValueBy(1M);
    }
}