using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class EternalFlame : MokouModCard
{
    public EternalFlame() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(typeof(RedCinder));
        WithTip(typeof(YellowCinder));
        WithTip(typeof(BlackCinder));
        WithTip(MokouModKeywords.Fuel);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EternalFlamePower>(this, 1M);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}