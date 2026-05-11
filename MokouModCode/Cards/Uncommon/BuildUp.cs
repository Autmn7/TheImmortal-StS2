using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class BuildUp : MokouModCard
{
    public BuildUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<VigorPower>(2));
        WithKeyword(MokouModKeywords.Fury);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BuildUpPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        if (FuryActive)
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
    }
}