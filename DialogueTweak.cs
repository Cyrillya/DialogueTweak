using DialogueTweak.Interfaces.UI.GUIChat;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak
{
    public class DialogueTweak : Mod
    {
        // 覆盖Icon的List，有一些默认值
        internal static List<IconInfo> IconInfos = new List<IconInfo>() {
            new IconInfo(IconType.Shop, NPCID.Guide, "DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Help"),
            new IconInfo(IconType.Extra, NPCID.Guide, "DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Hammer"),
            new IconInfo(IconType.Shop, NPCID.OldMan, "DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Old_Man"),
            new IconInfo(IconType.Shop, NPCID.TaxCollector, "Head"),
            new IconInfo(IconType.Shop, NPCID.Angler, "Head"),
            new IconInfo(IconType.Shop, NPCID.Nurse, "Head"),
        };
        public static Texture2D DefaultIcon;
        public static Texture2D SignIcon;
        public static Texture2D EditIcon;
        public override void PostSetupContent() {
            base.PostSetupContent();
            if (Main.netMode != NetmodeID.Server) {
                ButtonHandler.Button_Back = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Back");
                ButtonHandler.Button_BackLong = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_BackLong");
                ButtonHandler.Button_Happiness = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Happiness");
                ButtonHandler.Button_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Highlight");

                ButtonHandler.ButtonLong = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLong");
                ButtonHandler.ButtonLong_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLong_Highlight");
                ButtonHandler.ButtonLonger = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLonger");
                ButtonHandler.ButtonLonger_Highlight = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLonger_Highlight");

                ButtonHandler.Shop = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");
                ButtonHandler.Extra = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");

                DefaultIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");
                SignIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Sign");
                EditIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Edit");

                GUIChat.GreyPixel = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/GreyPixel");
                GUIChat.PortraitPanel = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/PortraitPanel");
                GUIChat.ChatStringBack = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/ChatStringBack");
            }
        }

        public GUIChat MobileChat = new GUIChat();

        public override void Load() {
            base.Load();
			instance = this;
			On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
        }

        // 通过调整screenWidth使一切绘制到屏幕之外，NPC对话机制不会被影响
        private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
            // 确保是处于NPC对话状态（PC版中编辑告示牌什么的也是这个UI）
            MobileChat.GUIDrawInner();
		}

		internal static DialogueTweak instance;

		public override void Unload() {
			instance = null;
		}

		public override object Call(params object[] args) {
			try {
				if (args is null) {
					throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
				}

				if (args.Length == 0) {
					throw new ArgumentException("Arguments cannot be empty!");
				}

				if (args[0] is string msg) {
					switch (msg) {
						case "ReplaceExtraButtonIcon":
							if (args.Length <= 3) {
								IconInfos.Add(new IconInfo(
									IconType.Extra, // This icon is for extra button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									));
							}
							else {
								IconInfos.Add(new IconInfo(
									IconType.Extra, // This icon is for extra button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									args[3] as Func<bool> // Available
									));
							}
							return true;
						case "ReplaceShopButtonIcon":
							if (args.Length <= 3) {
								IconInfos.Add(new IconInfo(
									IconType.Shop, // This icon is for shop button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									));
							}
							else {
								IconInfos.Add(new IconInfo(
									IconType.Shop, // This icon is for shop button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									args[3] as Func<bool> // Available
								));
							}
							return true;
						default:
							Logger.Error($"Replacement type \"{msg}\" not found.");
							return false;
					}
				}
			}
			catch (Exception e) {
				Logger.Error($"{e.StackTrace} {e.Message}");
			}

			return false;
		}
	}
}