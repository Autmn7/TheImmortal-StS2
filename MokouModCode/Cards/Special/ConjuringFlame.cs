using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class ConjuringFlame : MokouModCard
{
    private decimal _burnAmt;

    public ConjuringFlame() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithVars(new PowerVar<BurnPower>(BurnAmt));
        WithKeywords(CardKeyword.Exhaust);
    }

    public decimal BurnAmt
    {
        get => _burnAmt;
        set
        {
            _burnAmt = value;
            if (DynamicVars.ContainsKey("BurnPower")) DynamicVars["BurnPower"].BaseValue = value;
        }
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.75f));
        await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, BurnAmt, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}