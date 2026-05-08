using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class RiseFromAshes : MokouModCard
{
    public RiseFromAshes() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PowerVar<BurnPower>(10), new PowerVar<RekindlePower>(1));
        WithKeywords(CardKeyword.Exhaust);
    }

    public override Character.MokouMod.Animation Anim => Character.MokouMod.Animation.SpellChannel;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurningVfx.Create(Owner.Creature, 0.75f, false));
        await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<RekindlePower>(choiceContext, Owner.Creature, DynamicVars["RekindlePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}