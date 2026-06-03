using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
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
        WithTip(new TooltipSource(card => new HoverTip(new LocString("cards", Id.Entry + ".extraTipTitle"), new LocString("cards", Id.Entry + ".extraTipDescription"))));
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RekindlePower>(choiceContext, Owner.Creature, DynamicVars["RekindlePower"].BaseValue, Owner.Creature, this);
        var coveredForKeine = false;
        foreach (var creature in CombatState.GetTeammatesOf(Owner.Creature).Where(c => c != null && c.IsAlive && c.IsPlayer && c != Owner.Creature))
        {
            await PowerCmd.Apply<CoveredPower>(choiceContext, creature, 1M, Owner.Creature, this);
            if (creature.Player?.Character.Id.ToString() == "CHARACTER.KEINEMOD-KEINE_MOD")
                coveredForKeine = true;
        }

        await PowerCmd.Apply<RecklessSacrificesReflectionPower>(choiceContext, Owner.Creature, coveredForKeine ? 2 : 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}