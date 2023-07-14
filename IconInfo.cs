using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak;

internal enum IconType
{
    Happiness,
    Back,
    Shop,
    Extra
}

internal class IconInfo
{
    private readonly Func<string> _textureInternal;
    internal readonly IconType IconType;
    internal readonly List<int> NPCTypes;
    internal string Texture => _textureInternal() ?? "";
    internal Func<bool> Available;
    internal Func<Rectangle> Frame;
    internal Func<float> CustomOffset;

    internal IconInfo(IconType iconType, List<int> npcTypes, Func<string> texture) {
        IconType = iconType;
        NPCTypes = npcTypes ?? new List<int> {NPCID.None};
        _textureInternal = texture;
        if (!Main.dedServ && !ModContent.HasAsset(Texture) && Texture != "" &&
            ((iconType is IconType.Shop or IconType.Extra &&
              Texture is not "Head") || // 是Shop, Extra图标，但不是Head而且是给NPC用的
             iconType is IconType.Happiness or IconType.Back)) {
            // 是Happiness, Back
            DialogueTweak.Instance.Logger.Warn($"Texture path {Texture} is missing.");
        }

        Available = () => true;
        Frame = null;
        CustomOffset = null;
    }
}