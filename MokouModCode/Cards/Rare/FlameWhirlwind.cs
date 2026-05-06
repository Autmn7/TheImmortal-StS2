using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MokouMod.MokouModCode.Powers;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Cards.Rare;

public class FlameWhirlwind : MokouModCard
{
    public FlameWhirlwind() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(6);
        WithVars(new IgniteVar(25M));
        WithKeywords(MokouModKeywords.Ignite);
        WithTip(typeof(BurnPower));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayMokou(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var xValue = ResolveEnergyXValue();
        if (IgniteActive)
            xValue *= 2;
        if (xValue > 0)
        {
            var color = new Color("#c81e1e");
            var num2 = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.2 : 0.3;
            var instance1 = NCombatRoom.Instance;
            if (instance1 != null)
                instance1.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + Mathf.Min(8, xValue) * num2));
            SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
            var instance2 = NRun.Instance;
            if (instance2 != null) instance2.GlobalUi.AddChildSafely((Node)NSmokyVignetteVfx.Create(color, color));
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(xValue).FromCard(this)
            .TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_giant_horizontal_slash").Execute(choiceContext);
        if (xValue > 0)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
            await PowerCmd.Apply<BurnPower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
        DynamicVars["Ignite"].UpgradeValueBy(-5M);
    }
}