using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DialogueTweak
{
    public class Configuration : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("$Mods.DialogueTweak.Config.Scroll")]
        public bool TextScrolling;
    }
}