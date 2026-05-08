using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class Flare : MokouModCard
{
    public Flare() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<BurnPower>(2), new CardsVar("Feathers", 2));
        WithTip(new TooltipSource(card => HoverTipFactory.FromCard<Feather>(card.IsUpgraded)));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellCast;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
        var inHand = await Feather.CreateInHand(Owner, DynamicVars["Feathers"].IntValue, CombatState);
        if (!IsUpgraded)
            return;
        foreach (var card in inHand)
            CardCmd.Upgrade(card);
    }
}