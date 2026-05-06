using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Enchantments;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class ValiantHeart : MokouModCard
{
    public ValiantHeart() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(typeof(VigorousEnchantment));
        WithTip(typeof(VigorPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ValiantHeartPower>(this, 1M);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}