using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MokouMod.MokouModCode.Extensions;

namespace MokouMod.MokouModCode.Character;

public class MokouModCardPool : CustomCardPoolModel
{
    public override string Title => MokouMod.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    //Color of small card icons
    public override Color DeckEntryCardColor => new("#c81e1e");

    public override bool IsColorless => false;

    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load MokouMod/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/
    public override Texture2D? CustomFrame(CustomCardModel card)
    {
        return card.Type switch
        {
            CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("bg_attack_mokou.png".CharacterUiPath()),
            CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("bg_power_mokou.png".CharacterUiPath()),
            _ => PreloadManager.Cache.GetAsset<Texture2D>("bg_skill_mokou.png".CharacterUiPath())
        };
    }
}