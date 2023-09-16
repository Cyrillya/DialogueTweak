using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
            new(IconType.Extra, new List<int> { NPCID.DD2Bartender }, () => $"Terraria/Images/Item_{ItemID.DD2ElderCrystal}"),
            new(IconType.Extra, new List<int> { NPCID.Painter }, () => $"Terraria/Images/Item_{ItemID.FirstEncounter}"),
            new(IconType.Shop, new List<int> { NPCID.OldMan }, () => $"DialogueTweak/{ModAsset.Icon_Old_ManPath}"),
            new(IconType.Shop, new List<int> { NPCID.Nurse, NPCID.TaxCollector }, () => "Head"),
            new(IconType.Shop, new List<int> { NPCID.Angler }, () => {
                if (!Main.anglerQuestFinished && Main.anglerQuestItemNetIDs.IndexInRange(Main.anglerQuest) &&
                    TextureAssets.Item.IndexInRange(Main.anglerQuestItemNetIDs[Main.anglerQuest]))
                    return $"Terraria/Images/Item_{Main.anglerQuestItemNetIDs[Main.anglerQuest]}";
                return "Head";
            }),
            new(IconType.Extra, new List<int> { NPCID.PartyGirl }, () => !Main.swapMusic ? $"Terraria/Images/Item_{ItemID.MusicBoxOWDay}" : $"Terraria/Images/Item_{ItemID.MusicBoxDayRemix}"),
            new(IconType.Extra, new List<int> { NPCID.Dryad }, () => "Terraria/Images/Projectile_995") {
                Frame = () => new Rectangle(6, 108, 24, 32),
                Available = () => Main.LocalPlayer.HeldItem?.type is ItemID.JojaCola
            }
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
