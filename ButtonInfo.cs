using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak;

internal class ButtonInfo
{
    private readonly Func<string> _texturePathInternal;
    internal readonly List<int> NPCTypes;
    internal readonly Func<string> ButtonText;
    internal string IconTexture => _texturePathInternal() ?? "";
    internal readonly Action HoverAction;
    internal Func<bool> Available;
    internal Func<Rectangle> Frame;
    internal Func<float> CustomOffset;

    internal bool Focused;

    internal ButtonInfo(List<int> npcTypes, Func<string> buttonText, Func<string> iconTexture, Action hoverAction) {
        NPCTypes = npcTypes ?? new List<int> {NPCID.None};
        ButtonText = buttonText;
        _texturePathInternal = iconTexture;
        HoverAction = hoverAction;
        if (!Main.dedServ && !ModContent.HasAsset(IconTexture) && IconTexture != "") {
            DialogueTweak.Instance.Logger.Warn($"Texture path {IconTexture} is missing.");
        }

        Available = () => true;
        Frame = null;
        CustomOffset = null;
    }
}