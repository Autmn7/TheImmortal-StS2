using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Overwhelm : MokouModCard
{
    public Overwhelm() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8);
        WithKeywords(MokouModKeywords.Fury);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.AttackKick;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackCommand = await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        if (FuryActive)
            await CreatureCmd.GainBlock(Owner.Creature, attackCommand.Results.Sum(results => results.Sum(r => r.UnblockedDamage)), ValueProp.Move,
                cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}