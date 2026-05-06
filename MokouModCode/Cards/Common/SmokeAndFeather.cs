using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class SmokeAndFeather : MokouModCard
{
    public SmokeAndFeather() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6);
        WithVars(new CardsVar("Feathers", 1), new IgniteVar(3M));
        WithKeyword(MokouModKeywords.Ignite);
        WithTip(typeof(Feather));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await Feather.CreateInHand(Owner,
            IgniteActive ? DynamicVars["Feathers"].IntValue + 1 : DynamicVars["Feathers"].IntValue, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}