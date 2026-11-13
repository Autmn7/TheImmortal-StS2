using MegaCrit.Sts2.Core.Commands;
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
        WithVars(new DynamicVar("FanTheFlamesPower", 2M));
        WithTip(MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(BurnPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FanTheFlamesPower>(choiceContext, Owner.Creature, DynamicVars["FanTheFlamesPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FanTheFlamesPower"].UpgradeValueBy(1M);
    }
}