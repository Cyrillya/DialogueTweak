using System;
using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DialogueTweak;

internal class IconInfo
{
    private static readonly List<string> SpecialIconNames = [
        "Head",
        "QuestFish"
    ];

    public bool IsSpecialIcon => SpecialIconNames.Contains(Texture);

    private readonly Func<string> _textureInternal;
    internal readonly string InteractionTypeName;
    internal readonly List<int> NPCTypes;
    internal string Texture => _textureInternal() ?? "";
    internal Func<bool> Available;
    internal Func<Rectangle> Frame;
    internal Func<float> CustomOffset;

    internal IconInfo(string interactionTypeName, int npcType, string texture) : this(interactionTypeName, [npcType], texture) {
    }

    internal IconInfo(string interactionTypeName, List<int> npcTypes, string texture) : this(interactionTypeName, npcTypes, () => texture) {
    }

    internal IconInfo(string interactionTypeName, List<int> npcTypes, Func<string> texture) {
        InteractionTypeName = interactionTypeName;
        NPCTypes = npcTypes;
        _textureInternal = texture;
        if (!Main.dedServ && !ModContent.HasAsset(Texture) && Texture != "" && !IsSpecialIcon) {
            DialogueTweak.Instance.Logger.Warn($"Texture path {Texture} is missing.");
        }

        Available = () => true;
        Frame = null;
        CustomOffset = null;
    }

    public void GetIconParameters(out Func<float> shopCustomOffset, out Asset<Texture2D> texture,
        out Rectangle? texFrameOverride, int head) {
        texFrameOverride = null;
        shopCustomOffset = CustomOffset;
        switch (Texture) {
            case "Head":
                texture = ChatMethods.GetHeadOrDefaultIcon(head);
                break;
            case "QuestFish":
                if (!Main.anglerQuestFinished && Main.anglerQuestItemNetIDs.IndexInRange(Main.anglerQuest)) {
                    int fishId = Main.anglerQuestItemNetIDs[Main.anglerQuest];
                    if (TextureAssets.Item.IndexInRange(fishId)) {
                        texture = TextureAssets.Item[fishId];
                        break;
                    }
                }

                texture = ChatMethods.GetHeadOrDefaultIcon(head);
                break;
            default:
                texture = ModContent.Request<Texture2D>(Texture);
                texFrameOverride = Frame?.Invoke();
                break;
        }
    }
}
