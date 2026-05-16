using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class EternalFlame : MokouModCard
{
    public EternalFlame() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new DynamicVar("EternalFlamePower", 1));
        WithTip(typeof(RedCinder));
        WithTip(typeof(YellowCinder));
        WithTip(typeof(BlackCinder));
        WithTip(MokouModKeywords.Fuel);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EternalFlamePower>(choiceContext, Owner.Creature, DynamicVars["EternalFlamePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}