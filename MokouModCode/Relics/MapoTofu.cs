using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MokouMod.MokouModCode.Scripts;

namespace MokouMod.MokouModCode.Relics;

public class MapoTofu : MokouModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(5), new("Essence", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(MokouModKeywords.Essence)];

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        if (Owner.GetRelic<ImmortalPlume>() != null)
        {
            Owner.GetRelic<ImmortalPlume>()!.Flash();
            Owner.GetRelic<ImmortalPlume>()!.Essence += DynamicVars["Essence"].IntValue;
        }
        else if (Owner.GetRelic<BurningPlume>() != null)
        {
            Owner.GetRelic<BurningPlume>()!.Flash();
            Owner.GetRelic<BurningPlume>()!.Essence += DynamicVars["Essence"].IntValue;
        }
    }
}