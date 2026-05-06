using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MokouMod.MokouModCode.Extensions;

namespace MokouMod.MokouModCode.Powers;

public abstract class MokouModPower : CustomPowerModel
{
    //Loads from MokouMod/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}