using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Disintegrate : MokouModCard
{
    public Disintegrate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVars(new PowerVar<BurnPower>(3), new PowerVar<VulnerablePower>(1));
        WithKeywords(CardKeyword.Innate, MokouModKeywords.Fury, CardKeyword.Exhaust);
        WithTip(typeof(ArtifactPower));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target.HasPower<ArtifactPower>())
            await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
        if (FuryActive)
            await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this, DynamicVars.Vulnerable.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BurnPower"].UpgradeValueBy(2M);
        DynamicVars.Vulnerable.UpgradeValueBy(1M);
    }
}