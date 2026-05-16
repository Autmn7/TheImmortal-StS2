using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class AgonysEmbrace : MokouModCard
{
    public AgonysEmbrace() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new DynamicVar("AgonysEmbracePower", 1));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AgonysEmbracePower>(choiceContext, Owner.Creature, DynamicVars["AgonysEmbracePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}