using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.CardPools;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class MokousSuppression : MokouModCard, DualSuppression.IChoosable
{
    public MokousSuppression() : base(-1, CardType.Skill, CardRarity.Event, TargetType.None)
    {
        WithPower<BurnPower>(10);
    }

    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    public async Task OnChosen()
    {
        Log.Info("Check: " + (CombatState == null));
        await PowerCmd.Apply<BurnPower>(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState.HittableEnemies, DynamicVars["BurnPower"].BaseValue, Owner.Creature, this);
    }
}