global using DialogueTweak.Interfaces;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using ReLogic.Content;
global using ReLogic.Graphics;
global using System;
global using System.Collections.Generic;
global using System.Linq;
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
// C# 10.0新加的global using https://zhuanlan.zhihu.com/p/433239269

namespace DialogueTweak
{
    public partial class DialogueTweak : Mod
    {
		internal static DialogueTweak instance;
		internal static GUIChatDraw MobileChat;

		public override void Load() {
			instance = this;
			On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
		}

		public override void Unload() {
			instance = null;
        }

		// 通过调整screenWidth使一切绘制到屏幕之外，NPC对话机制不会被影响
		private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
			// 确保是处于NPC对话状态（PC版中编辑告示牌什么的也是这个UI）
			GUIChatDraw.GUIDrawInner();
		}
	}
}