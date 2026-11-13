using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class TragicFate : MokouModCard
{
    public TragicFate() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DoubleDamagePower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
        await PowerCmd.Apply<TragicFatePower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}