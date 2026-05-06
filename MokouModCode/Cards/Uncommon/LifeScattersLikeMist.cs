using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class LifeScattersLikeMist : MokouModCard
{
    public LifeScattersLikeMist() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");
        VfxCmd.PlayOnCreature(Owner.Creature, "vfx/vfx_attack_lightning");
        await CreatureCmd.Kill(Owner.Creature);
        await CardPileCmd.Draw(choiceContext, 10 - Owner.PlayerCombatState.Hand.Cards.Count, Owner);
        await CardPileCmd.Draw(choiceContext, 10 - cardPlay.Target.Player.PlayerCombatState.Hand.Cards.Count,
            cardPlay.Target.Player);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}