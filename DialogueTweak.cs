global using DialogueTweak.Interfaces.UI.GUIChat;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using ReLogic.Content;
global using ReLogic.Graphics;
global using System;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.GameContent.UI.States;
global using Terraria.GameInput;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using Terraria.UI;
global using Terraria.UI.Chat;
global using Terraria.UI.Gamepad;
// C# 10.0ÐÂ¼ÓµÄglobal using https://zhuanlan.zhihu.com/p/433239269

namespace DialogueTweak
{
    public class DialogueTweak : Mod
    {
		internal static DialogueTweak instance;

        public override void Load() {
			instance = this;
		}

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
								HandleAssets.IconInfos.Add(new IconInfo(
									IconType.Extra, // This icon is for extra button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									));
							}
							else {
								HandleAssets.IconInfos.Add(new IconInfo(
									IconType.Extra, // This icon is for extra button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									args[3] as Func<bool> // Available
									));
							}
							return true;
						case "ReplaceShopButtonIcon":
							if (args.Length <= 3) {
								HandleAssets.IconInfos.Add(new IconInfo(
									IconType.Shop, // This icon is for shop button.
									Convert.ToInt32(args[1]), // NPC ID
									args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
									));
							}
							else {
								HandleAssets.IconInfos.Add(new IconInfo(
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