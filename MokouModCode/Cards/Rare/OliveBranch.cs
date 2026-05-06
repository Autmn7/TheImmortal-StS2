using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class OliveBranch : MokouModFuelCard
{
    public OliveBranch() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        Durability = MaxDurability = 3M;
        WithVars(new DurabilityVar(3), new PowerVar<RekindlePower>(1), new PowerVar<RegenPower>(1));
        WithKeywords(CardKeyword.Unplayable, CardKeyword.Retain, MokouModKeywords.Fuel);
        WithTip(MokouModKeywords.Essence);
    }

    protected override async Task OnFuelTrigger()
    {
        await CommonActions.ApplySelf<TempEssencePower>(this, 1);
    }

    protected override async Task OnFuelDurabilityDeplete()
    {
        await PowerCmd.Apply<RekindlePower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars["RekindlePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<RegenPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
            DynamicVars["RegenPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RegenPower"].UpgradeValueBy(2M);
    }
}