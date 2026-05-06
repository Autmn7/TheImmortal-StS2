using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MokouMod.MokouModCode.Powers;

namespace MokouMod.MokouModCode.Cards.Rare;

public class Incinerate : MokouModCard
{
    private int _currentBurn = 1;
    private int _increasedBurn;

    public Incinerate() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithVars(new PowerVar<BurnPower>(CurrentBurn), new DynamicVar("Increment", 1M));
        WithKeyword(CardKeyword.Exhaust);
    }

    [SavedProperty]
    public int CurrentBurn
    {
        get => _currentBurn;
        set
        {
            AssertMutable();
            _currentBurn = value;
            DynamicVars["BurnPower"].BaseValue = _currentBurn;
        }
    }

    [SavedProperty]
    public int IncreasedBurn
    {
        get => _increasedBurn;
        set
        {
            AssertMutable();
            _increasedBurn = value;
        }
    }

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(cardPlay.Target, 0.75f));
        await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, DynamicVars["BurnPower"].BaseValue,
            Owner.Creature, this);
        var intValue = DynamicVars["Increment"].IntValue;
        BuffFromPlay(intValue);
        if (!(DeckVersion is Incinerate deckVersion))
            return;
        deckVersion.BuffFromPlay(intValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increment"].UpgradeValueBy(1M);
    }

    protected override void AfterDowngraded()
    {
        UpdateBurn();
    }

    private void BuffFromPlay(int extraBurn)
    {
        IncreasedBurn += extraBurn;
        UpdateBurn();
    }

    private void UpdateBurn()
    {
        CurrentBurn = 1 + IncreasedBurn;
    }
}