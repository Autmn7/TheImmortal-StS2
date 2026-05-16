using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class SiphoningFlame : MokouModCard
{
    public SiphoningFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVars(new DynamicVar("Ratio", 5), new IgniteVar(15M));
        WithKeywords(MokouModKeywords.Ignite, CardKeyword.Exhaust);
        WithTip(typeof(ConjuringFlame));
        WithTip(typeof(BurnPower));
        WithTip(typeof(RegenPower));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal burnRemoved = cardPlay.Target.GetPowerAmount<BurnPower>();
        var regenAmount = (int)Math.Floor(burnRemoved / DynamicVars["Ratio"].BaseValue);
        await PowerCmd.Remove(cardPlay.Target.GetPower<BurnPower>());
        if (regenAmount > 0)
            await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, regenAmount, Owner.Creature, this);
        var conjuringFlame = CombatState.CreateCard<ConjuringFlame>(Owner);
        conjuringFlame.BurnAmt = burnRemoved;
        if (IgniteActive)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(conjuringFlame, PileType.Discard, Owner));
            await Cmd.Wait(0.5f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Ratio"].UpgradeValueBy(-1M);
        DynamicVars["Ignite"].UpgradeValueBy(-3M);
    }
}