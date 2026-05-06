using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MokouMod.MokouModCode.Character;
using MokouMod.MokouModCode.Extensions;

namespace MokouMod.MokouModCode.Potions;

[Pool(typeof(MokouModPotionPool))]
public abstract class MokouModPotion : CustomPotionModel
{
    public override string CustomPackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : "potion.png".PotionImagePath();
        }
    }

    public override string CustomPackedOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : "potion_outline.png".PotionImagePath();
        }
    }
}