using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MokouMod.MokouModCode.Cards;

namespace MokouMod.MokouModCode.Scripts;

public class DurabilityVar : DynamicVar
{
    public const string DefaultName = "Durability";

    public DurabilityVar(decimal value) : base(DefaultName, value)
    {
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        if (card is MokouModFuelCard fuel) PreviewValue = fuel.Durability;
    }
}