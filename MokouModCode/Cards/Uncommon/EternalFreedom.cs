using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class EternalFreedom : MokouModCard
{
    public EternalFreedom() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(4);
        WithVars(new PowerVar<VigorPower>(1));
        WithKeywords(MokouModKeywords.Ember);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var card in GetCards().ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
            var num = await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            if (EmberActive)
                await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }

    private IEnumerable<CardModel> GetCards()
    {
        return PileType.Hand.GetPile(Owner).Cards
            .Where((Func<CardModel, bool>)(c => c.Type != CardType.Attack));
    }
}