using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Cards.Special;

namespace MokouMod.MokouModCode.Cards.Rare;

public class RegretfulLife : MokouModCard
{
    public RegretfulLife() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new HpLossVar(10M));
        WithKeyword(CardKeyword.Exhaust);
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<RecklessSacrifice>(card.IsUpgraded)));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nonLethal = CalculateNonLethal(DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, nonLethal, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        var card = CombatState.CreateCard<RecklessSacrifice>(Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}