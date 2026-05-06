using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MokouMod.MokouModCode.Cards.Ancient;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Basic;

public class SelfHarmingKick : MokouModCard, ITranscendenceCard
{
    public SelfHarmingKick() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithVars(new HpLossVar(2), new PowerVar<BurnPower>(3));
        WithKeyword(MokouModKeywords.Nonlethal);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<DevastatingKick>();
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nonLethal = CalculateNonLethal(DynamicVars.HpLoss.BaseValue);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, nonLethal,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.5f));
        await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }
}