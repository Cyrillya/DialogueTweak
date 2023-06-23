using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DialogueTweak;

[Label("$Mods.DialogueTweak.Configs.Configuration.DisplayName")]
public class Configuration : ModConfig
{
    public static Configuration Instance;
        
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)]
    [Label("$Mods.DialogueTweak.Configs.Configuration.TextScrolling.Label")]
    [Tooltip("$Mods.DialogueTweak.Configs.Configuration.TextScrolling.Tooltip")]
    public bool TextScrolling;

    [DefaultValue(true)]
    [Label("$Mods.DialogueTweak.Configs.Configuration.DisplayPreference.Label")]
    [Tooltip("$Mods.DialogueTweak.Configs.Configuration.DisplayPreference.Tooltip")]
    public bool DisplayPreference;

    [DefaultValue(false)]
    [Label("$Mods.DialogueTweak.Configs.Configuration.BestiryPortrait.Label")]
    [Tooltip("$Mods.DialogueTweak.Configs.Configuration.BestiryPortrait.Tooltip")]
    public bool BestiryPortrait;

    [DefaultValue(false)]
    [Label("$Mods.DialogueTweak.Configs.Configuration.VanillaUI.Label")]
    [Tooltip("$Mods.DialogueTweak.Configs.Configuration.VanillaUI.Tooltip")]
    public bool VanillaUI;
        
    public override void OnLoaded() {
        Instance = this;
    }
}