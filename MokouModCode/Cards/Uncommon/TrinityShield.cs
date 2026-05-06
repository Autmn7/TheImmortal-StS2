using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class TrinityShield : MokouModCard
{
    public TrinityShield() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(15);
        WithVars(new IgniteVar(6M));
        WithKeywords(MokouModKeywords.Ignite, MokouModKeywords.Fury, MokouModKeywords.Ember);
        WithEnergyTip();
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var energy = 0M;
        if (IgniteActive) energy += 1;
        if (FuryActive) energy += 1;
        if (EmberActive) energy += 1;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (energy > 0) await PlayerCmd.GainEnergy(energy, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
        AddKeyword(CardKeyword.Retain);
    }
}