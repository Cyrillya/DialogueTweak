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
        new IconInfo("GuideTip", NPCID.Guide, $"DialogueTweak/{ModAsset.Icon_Help_Path}"),
        new IconInfo("GuideReverseCrafting", NPCID.Guide, $"DialogueTweak/{ModAsset.Icon_Hammer_Path}"),
        new IconInfo("TavernkeepAdvice", NPCID.DD2Bartender, $"Terraria/Images/Item_{ItemID.DD2ElderCrystal}"),
        new IconInfo("OldManCurse", NPCID.OldMan, $"DialogueTweak/{ModAsset.Icon_Old_Man_Path}"),
        new IconInfo("AnglerQuest", NPCID.Angler, "QuestFish"),
        new IconInfo("StardewValleyBit", NPCID.Dryad, "Terraria/Images/Projectile_995") {
            Frame = () => new Rectangle(6, 108, 24, 32),
            Available = () => Main.LocalPlayer.HeldItem?.type is ItemID.JojaCola
        },
        new IconInfo("NurseHeal", [NPCID.Nurse], "Head"),
        new IconInfo("TaxCollectorCollectTaxes", [NPCID.TaxCollector], "Head"),
        new IconInfo("PartyGirlMusicSwap", [NPCID.PartyGirl],
            () => !Main.swapMusic
                ? $"Terraria/Images/Item_{ItemID.MusicBoxOWDay}"
                : $"Terraria/Images/Item_{ItemID.MusicBoxDayRemix}"),
    ];

    public override void PostSetupContent() {
        if (Main.netMode == NetmodeID.Server) return;

        ButtonHandler.ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
        ButtonHandler.ButtonPanel_Highlight =
            Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");


        ChatUI.ChatTextPanel = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
        ChatUI.BiomeIconTags = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow");
    }
}