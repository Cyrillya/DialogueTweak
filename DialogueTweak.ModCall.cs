using System;
using System.Collections.Generic;
using DialogueTweak.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DialogueTweak;

public partial class DialogueTweak : Mod
{
	private static void AddButtonReplacement(IconType iconType, params object[] args) {
		var iconInfo = new IconInfo(
			iconType, // This icon is for extra button.
			AsListOfInt(args[1]), // NPC IDs
			AsFuncString(args[2]) // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
		);
		if (args.Length > 3) {
			iconInfo.Available = args[3] as Func<bool>;
		}
		if (args.Length > 4) {
			iconInfo.Frame = args[4] as Func<Rectangle>;
		}
		if (args.Length > 5) {
			iconInfo.CustomOffset = args[4] as Func<float>;
		}
		HandleAssets.IconInfos.Add(iconInfo);
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
					case "ReplaceExtraButtonIcon": {
						AddButtonReplacement(IconType.Extra, args);
						return true;
					}
					case "ReplaceShopButtonIcon": {
						AddButtonReplacement(IconType.Shop, args);
						return true;
					}
					case "ReplaceHappinessButtonIcon": {
						AddButtonReplacement(IconType.Happiness, args);
						return true;
					}
					case "ReplaceBackButtonIcon": {
						AddButtonReplacement(IconType.Back, args);
						return true;
					}
					case "OnPostPortraitDraw": {
						PortraitDrawer.OnPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle>;
						return true;
					}
					case "OnPreNPCPortraitDraw": {
						PortraitDrawer.OnPreNPCPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, NPC>;
						return true;
					}
					case "OnPostNPCPortraitDraw": {
						PortraitDrawer.OnPostNPCPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, NPC>;
						return true;
					}
					case "OnPreSignPortraitDraw": {
						PortraitDrawer.OnPreSignPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, int>;
						return true;
					}
					case "OnPostSignPortraitDraw": {
						PortraitDrawer.OnPostSignPortraitDraw += args[1] as Action<SpriteBatch, Color, Rectangle, int>;
						return true;
					}
					case "AddButton": {
						var buttonInfo = new ButtonInfo(
							AsListOfInt(args[1]), // NPC IDs
							args[2] as Func<string>, // NPC Button Text
							AsFuncString(args[3]), // Icon Texture Path (With Mod Name)
							args[4] as Action // Hover Action
						);
						if (args.Length > 5) {
							buttonInfo.Available = args[5] as Func<bool>;
						}
						if (args.Length > 6) {
							buttonInfo.Frame = args[6] as Func<Rectangle>;
						}
						if (args.Length > 7) {
							buttonInfo.CustomOffset = args[7] as Func<float>;
						}
						HandleAssets.ButtonInfos.Add(buttonInfo);
						return true;
					}
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

	private static List<int> AsListOfInt(object data) => data is List<int> ? data as List<int> : new List<int>() { Convert.ToInt32(data) };

	private static Func<string> AsFuncString(object data) => data is Func<string> ? data as Func<string> : () => data as string;
}