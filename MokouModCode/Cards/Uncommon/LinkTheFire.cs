using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class LinkTheFire : MokouModCard
{
    public LinkTheFire() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<BurnPower>(2);
        WithPower<RegenPower>(3);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(new TooltipSource(card => new HoverTip(new LocString("cards", Id.Entry + ".extraTipTitle"), new LocString("cards", Id.Entry + ".extraTipDescription"))));
        WithCostUpgradeBy(-1);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var creature in CombatState.GetTeammatesOf(Owner.Creature).Where(c => c.IsAlive && c.IsPlayer))
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurningVfx.Create(creature, 0.75f, false));
            if (creature.Player?.Character.Id.ToString() != "CHARACTER.KEINEMOD-KEINE_MOD" || creature == Owner.Creature)
                await PowerCmd.Apply<BurnPower>(choiceContext, creature, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<RegenPower>(choiceContext, creature, DynamicVars["RegenPower"].BaseValue, Owner.Creature, this);
        }
    }
}