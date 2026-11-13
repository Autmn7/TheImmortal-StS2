using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class CurseOfIwakasa : MokouModCard
{
    public CurseOfIwakasa() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip(typeof(Hellfire));
        WithTip(typeof(HellfirePower));
        WithTip(MokouModKeywords.Fuel);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CurseOfIwakasaPower>(choiceContext, cardPlay.Target, 1M, Owner.Creature, this);
        var card = CombatState.CreateCard<Hellfire>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}