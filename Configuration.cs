using System.ComponentModel;
using DialogueTweak.Interfaces;
using Terraria.ModLoader.Config;

namespace DialogueTweak;

public class Configuration : ModConfig
{
    public enum TextScrollingSpeed : int
    {
        Slow,
        Regular,
        Fast,
        Disabled
    }

    public static Configuration Instance;
        
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(TextScrollingSpeed.Regular)]
    [DrawTicks]
    public TextScrollingSpeed TextScrollingMode;

    [DefaultValue(true)]
    public bool DisplayPreference;

    [DefaultValue(true)]
    public bool DisplayHappiness;

    [DefaultValue(false)]
    public bool VanillaUI;

    [DefaultValue(true)]
    public bool ShowSwapButton;

    [DefaultValue(true)]
    public bool PortraitAnimation;

    public override void OnLoaded() {
        Instance = this;
    }
}