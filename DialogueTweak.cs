global using DialogueTweak.Interfaces;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;
global using ReLogic.Content;
global using ReLogic.Graphics;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.GameContent.Personalities;
global using Terraria.GameContent.UI.States;
global using Terraria.GameInput;
global using Terraria.Graphics.Shaders;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using Terraria.UI;
global using Terraria.UI.Chat;
global using Terraria.UI.Gamepad;

namespace DialogueTweak;

public partial class DialogueTweak : Mod
{
    internal static DialogueTweak Instance;

    public override void Load() {
        Instance = this;
    }

    public override void Unload() {
        Instance = null;
        Configuration.Instance = null;
    }
}