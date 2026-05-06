using BaseLib.Abstracts;
using Godot;
using MokouMod.MokouModCode.Extensions;

namespace MokouMod.MokouModCode.Character;

public class MokouModPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => MokouMod.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}