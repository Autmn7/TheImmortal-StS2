using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class TauntMokou : MokouModCard
{
    public TauntMokou() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithBlock(12);
        WithVars(new DynamicVar("TempStrength", 2M), new PowerVar<StrengthPower>(1));
        WithKeywords(MokouModKeywords.Fury);
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CommonActions.Apply<TauntPower>(cardPlay.Target, this, DynamicVars["TempStrength"].IntValue);
        if (FuryActive)
            await CommonActions.Apply<StrengthPower>(cardPlay.Target, this, -DynamicVars.Strength.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4M);
    }
}