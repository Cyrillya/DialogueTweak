using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DialogueTweak;

public class Configuration : ModConfig
{
    public static Configuration Instance;
        
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)]
    public bool TextScrolling;

    [DefaultValue(true)]
    public bool DisplayPreference;

    [DefaultValue(false)]
    public bool BestiryPortrait;

    [DefaultValue(false)]
    public bool VanillaUI;
        
    public override void OnLoaded() {
        Instance = this;
    }
}