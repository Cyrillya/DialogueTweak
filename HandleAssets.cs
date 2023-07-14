using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak
{
    // 一个贴图库，使用贴图时可以直接从这里调用
    internal class HandleAssets : ModSystem
    {
        // 覆盖Icon的List，有一些默认值
        internal static List<IconInfo> IconInfos = new List<IconInfo>() {
            new(IconType.Shop, new List<int> { NPCID.Guide }, () => $"DialogueTweak/{ModAsset.Icon_HelpPath}"),
            new(IconType.Extra, new List<int> { NPCID.Guide }, () => $"DialogueTweak/{ModAsset.Icon_HammerPath}"),
            new(IconType.Shop, new List<int> { NPCID.OldMan }, () => $"DialogueTweak/{ModAsset.Icon_Old_ManPath}"),
            new(IconType.Shop, new List<int> { NPCID.Nurse, NPCID.Angler, NPCID.TaxCollector }, () => "Head")
        };
        internal static List<ButtonInfo> ButtonInfos = new();
        public override void PostSetupContent() {
            base.PostSetupContent();
            if (Main.netMode != NetmodeID.Server) {
                ButtonHandler.ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
                ButtonHandler.ButtonPanel_Highlight = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

                ButtonHandler.Shop = ModAsset.Icon_Default;
                ButtonHandler.Extra = ModAsset.Icon_Default;

                ChatUI.ChatTextPanel = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
                ChatUI.BiomeIconTags = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow");
            }
        }
    }
}
