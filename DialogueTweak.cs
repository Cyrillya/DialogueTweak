using System;
using System.Collections.Generic;
using DialogueTweak.CrossModHelper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

    // Test code
    /*
    public override void PostSetupContent() {
        DprHelper.AddButton(
            NPCID.Angler,
            () => "Sandstorm (1 Gold)",
            () => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow",
            () => {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    Main.NewText("这是假的关闭按钮哦");
            },
            () => true,
            () => {
                if (Main.LocalPlayer.direction == -1)
                    return new Rectangle(0, 0, 22, 22);
                return new Rectangle(10, 10, 11, 11);
            },
            () => {
                return 44f;
            });
    }
    */
}