using System.ComponentModel;
using DialogueTweak.Interfaces;
using Terraria.ModLoader.Config;

namespace DialogueTweak;

public class Configuration : ModConfig
{
    public enum PortraitStyle : int
    {
        LiveReaction,
        Static,
        Bestiary
    }
    
    public static Configuration Instance;
        
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)]
    public bool TextScrolling;

    [DefaultValue(true)]
    public bool DisplayPreference;

    [DefaultValue(false)]
    public bool VanillaUI;

    [DefaultValue(true)]
    public bool ShowSwapButton;

    [DefaultValue(PortraitStyle.LiveReaction)]
    [DrawTicks]
    public PortraitStyle PortraitDrawStyle;
        
    public override void OnLoaded() {
        Instance = this;
    }

    public override void OnChanged() {
        PortraitDrawer.EntryIcon = null; // Refresh
    }
}