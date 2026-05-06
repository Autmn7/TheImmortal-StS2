using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Special;

[Pool(typeof(TokenCardPool))]
public class Feather : MokouModCard
{
    public Feather() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithVars(new PowerVar<BurnPower>(2M), new DynamicVar("AdditionalBurn", 1M), new IgniteVar(5M));
        WithKeywords(MokouModKeywords.Ignite, MokouModKeywords.Fury, MokouModKeywords.Ember, CardKeyword.Exhaust);
    }

    public override TargetType TargetType => !HasHeatwave ? TargetType.AnyEnemy : TargetType.AllEnemies;

    private bool HasHeatwave => IsMutable && Owner != null && Owner.Creature.HasPower<HeatwavePower>();

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.5f));
        var burnAmount = DynamicVars["BurnPower"].BaseValue;
        if (IgniteActive || FuryActive || EmberActive)
            burnAmount += DynamicVars["AdditionalBurn"].BaseValue;
        if (HasHeatwave)
            await PowerCmd.Apply<BurnPower>(choiceContext, CombatState.HittableEnemies,
                burnAmount, Owner.Creature, this);
        else
            await CommonActions.Apply<BurnPower>(cardPlay.Target, this, burnAmount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BurnPower"].UpgradeValueBy(1M);
    }

    public static async Task<CardModel?> CreateInHand(Player owner, ICombatState combatState)
    {
        return (await CreateInHand(owner, 1, combatState)).FirstOrDefault<CardModel>();
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState)
    {
        if (count == 0)
            return Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();
        var feathers = new List<CardModel>();
        for (var index = 0; index < count; ++index)
            feathers.Add(combatState.CreateCard<Feather>(owner));
        var combat =
            await CardPileCmd.AddGeneratedCardsToCombat(feathers, PileType.Hand, owner);
        return feathers;
    }
}