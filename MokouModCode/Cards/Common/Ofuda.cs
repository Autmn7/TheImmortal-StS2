using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Common;

public class Ofuda : MokouModFuelCard
{
    public Ofuda() : base(-1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        Durability = MaxDurability = 2M;
        WithVars(new DurabilityVar(2), new PowerVar<BurnPower>(2), new DynamicVar("StrengthLoss", 3M));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(StrengthPower));
    }

    protected override async Task OnFuelTrigger()
    {
        foreach (var target in CombatState.HittableEnemies)
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely((Node)NFireBurstVfx.Create(target, 0.5f));
        await PowerCmd.Apply<BurnPower>(new ThrowingPlayerChoiceContext(),
            CombatState.HittableEnemies, DynamicVars["BurnPower"].IntValue, Owner.Creature,
            this);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext context, CardModel card, bool causedByEthereal)
    {
        if (card == this)
            await PowerCmd.Apply<OfudaPower>(context, CombatState.HittableEnemies, DynamicVars["StrengthLoss"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(2M);
    }
}