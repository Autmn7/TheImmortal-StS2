using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class LifeScattersLikeMist : MokouModCard
{
    public LifeScattersLikeMist() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.None;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");
        VfxCmd.PlayOnCreature(Owner.Creature, "vfx/vfx_attack_lightning");
        await CreatureCmd.Kill(Owner.Creature);
        if (Owner.PlayerCombatState != null)
            await CardPileCmd.Draw(choiceContext, CardPile.MaxCardsInHand - Owner.PlayerCombatState.Hand.Cards.Count, Owner);
        if (cardPlay.Target.Player?.PlayerCombatState != null)
            await CardPileCmd.Draw(choiceContext, CardPile.MaxCardsInHand - cardPlay.Target.Player.PlayerCombatState.Hand.Cards.Count, cardPlay.Target.Player);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}