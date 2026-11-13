using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class FearNoDeath : MokouModCard
{
    public FearNoDeath() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("FearNoDeathPower", 3M));
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FearNoDeathPower>(choiceContext, Owner.Creature, DynamicVars["FearNoDeathPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FearNoDeathPower"].UpgradeValueBy(1M);
    }
}