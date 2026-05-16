using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Cards.Special;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Uncommon;

public class PhoenixsFeathers : MokouModCard
{
    public PhoenixsFeathers() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new DynamicVar("PhoenixsFeathersPower", 1));
        WithTip(typeof(Feather));
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PhoenixsFeathersPower>(choiceContext, Owner.Creature, DynamicVars["PhoenixsFeathersPower"].BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}