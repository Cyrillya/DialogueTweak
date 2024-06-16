using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak;

// 一个贴图库，使用贴图时可以直接从这里调用
internal class HandleAssets : ModSystem
{
    // 覆盖Icon的List，有一些默认值
    internal static List<IconInfo> IconInfos = [
        new IconInfo(IconType.Shop, NPCID.Guide, $"DialogueTweak/{ModAsset.Icon_Help_Path}"),
        new IconInfo(IconType.Extra, NPCID.Guide, $"DialogueTweak/{ModAsset.Icon_Hammer_Path}"),
        new IconInfo(IconType.Extra, NPCID.DD2Bartender, $"Terraria/Images/Item_{ItemID.DD2ElderCrystal}"),
        new IconInfo(IconType.Extra, NPCID.Painter, $"Terraria/Images/Item_{ItemID.FirstEncounter}"),
        new IconInfo(IconType.Shop, NPCID.OldMan, $"DialogueTweak/{ModAsset.Icon_Old_Man_Path}"),
        new IconInfo(IconType.Shop, NPCID.Angler, "QuestFish"),
        new IconInfo(IconType.Extra, NPCID.Dryad, "Terraria/Images/Projectile_995") {
            Frame = () => new Rectangle(6, 108, 24, 32),
            Available = () => Main.LocalPlayer.HeldItem?.type is ItemID.JojaCola
        },

        new IconInfo(IconType.Shop, [NPCID.Nurse, NPCID.TaxCollector], "Head"),
        new IconInfo(IconType.Extra, [NPCID.PartyGirl],
            () => !Main.swapMusic
                ? $"Terraria/Images/Item_{ItemID.MusicBoxOWDay}"
                : $"Terraria/Images/Item_{ItemID.MusicBoxDayRemix}"),
    ];

    internal static List<ButtonInfo> ButtonInfos = [];

    public override void PostSetupContent() {
        if (Main.netMode == NetmodeID.Server) return;

        ButtonHandler.ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
        ButtonHandler.ButtonPanel_Highlight =
            Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

        ButtonHandler.Shop = ModAsset.Icon_Default;
        ButtonHandler.Extra = ModAsset.Icon_Default;

        ChatUI.ChatTextPanel = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
        ChatUI.BiomeIconTags = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow");
    }
}