using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class PhoenixForm : MokouModCard
{
    public PhoenixForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new DynamicVar("PhoenixFormPower", 6M));
        WithTip(typeof(BurnPower));
        WithTip(typeof(RekindlePower));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.Resurrection;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PhoenixFormPower>(choiceContext, Owner.Creature, DynamicVars["PhoenixFormPower"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PhoenixFormPower"].UpgradeValueBy(3M);
    }
}