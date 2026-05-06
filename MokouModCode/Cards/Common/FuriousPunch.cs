using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class FuriousPunch : MokouModCard
{
    public FuriousPunch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithVars(new PowerVar<VulnerablePower>(1M), new PowerVar<VulnerablePower>("AdditionalVul", 1M));
        WithKeywords(MokouModKeywords.Fury);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this,
            FuryActive
                ? DynamicVars.Vulnerable.BaseValue + DynamicVars["AdditionalVul"].BaseValue
                : DynamicVars.Vulnerable.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["AdditionalVul"].UpgradeValueBy(1M);
    }
}