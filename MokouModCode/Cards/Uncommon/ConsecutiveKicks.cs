using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class ConsecutiveKicks : MokouModCard
{
    public ConsecutiveKicks() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(2);
        WithVar(new RepeatVar(3));
        WithVar("ExtraTime", 1, 1);
        WithKeywords(MokouModKeywords.Fury);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackAirKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitCount(FuryActive ? DynamicVars.Repeat.IntValue + DynamicVars["ExtraTime"].IntValue : DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1M);
    }
}