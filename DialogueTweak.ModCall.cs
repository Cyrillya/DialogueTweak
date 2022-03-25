using System.Collections.Generic;

namespace DialogueTweak
{
    public partial class DialogueTweak : Mod
	{
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
								if (args.Length <= 3) {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Extra, // This icon is for extra button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										));
								}
								else {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Extra, // This icon is for extra button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										args[3] as Func<bool>, // Available
										args[4] as Func<Rectangle> // Frame Rectangle
										));
								}
								return true;
							}
						case "ReplaceShopButtonIcon": {
								if (args.Length <= 3) {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Shop, // This icon is for shop button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										));
								}
								else {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Shop, // This icon is for shop button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										args[3] as Func<bool>, // Available
										args[4] as Func<Rectangle> // Frame Rectangle
										));
								}
								return true;
							}
						case "ReplaceHappinessButtonIcon": {
								if (args.Length <= 3) {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Happiness, // This icon is for happiness button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										));
								}
								else {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Happiness, // This icon is for happiness button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										args[3] as Func<bool>, // Available
										args[4] as Func<Rectangle> // Frame Rectangle
										));
								}
								return true;
							}
						case "ReplaceBackButtonIcon": {
								if (args.Length <= 3) {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Back, // This icon is for back button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										));
								}
								else {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Back, // This icon is for back button.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string, // Texture Path (With Mod Name) ("Head" for overriding icon to the NPC's head.)
										args[3] as Func<bool>, // Available
										args[4] as Func<Rectangle> // Frame Rectangle
										));
								}
								return true;
							}
						case "ReplacePortrait": {
								if (args.Length <= 3) {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Portrait, // This icon is for portrait.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string // Texture Path (With Mod Name) ("None" for not drawing portrait.)
										));
								}
								else {
									HandleAssets.IconInfos.Add(new IconInfo(
										IconType.Portrait, // This icon is for portrait.
										AsListOfInt(args[1]), // NPC IDs
										args[2] as string, // Texture Path (With Mod Name) ("None" for not drawing portrait.)
										args[3] as Func<bool>, // Available
										args[4] as Func<Rectangle> // Frame Rectangle
										));
								}
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
								if (args.Length <= 5) {
									HandleAssets.ButtonInfos.Add(new ButtonInfo(
										AsListOfInt(args[1]), // NPC IDs
										args[2] as Func<string>, // NPC Button Text
										args[3] as string, // Icon Texture Path (With Mod Name)
										args[4] as Action // Hover Action
										));
								}
								else {
									HandleAssets.ButtonInfos.Add(new ButtonInfo(
										AsListOfInt(args[1]), // NPC IDs
										args[2] as Func<string>, // NPC Button Text
										args[3] as string, // Icon Texture Path (With Mod Name)
										args[4] as Action, // Hover Action
										args[5] as Func<bool>, // Available
										args[6] as Func<Rectangle> // Frame Rectangle
										));
								}
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

            static List<int> AsListOfInt(object data) => data is List<int> ? data as List<int> : new List<int>() { Convert.ToInt32(data) };

			return false;
		}
	}
}
