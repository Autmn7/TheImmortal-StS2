using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class RecklessSacrificesReflection : MokouModCard
{
    public RecklessSacrificesReflection() : base(4, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithVars(new PowerVar<RekindlePower>(1));
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RekindlePower>(choiceContext, Owner.Creature, DynamicVars["RekindlePower"].BaseValue, Owner.Creature, this);
        foreach (var player in Owner.Creature.CombatState.Players)
            if (player != Owner)
                await PowerCmd.Apply<CoveredPower>(choiceContext, player.Creature, 1M, Owner.Creature, this);
        await CommonActions.ApplySelf<RecklessSacrificesReflectionPower>(this, 1M);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}