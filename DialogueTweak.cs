using DialogueTweak.Interfaces.UI.GUIChat;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DialogueTweak
{
    public class DialogueTweak : Mod
    {
        public static Texture2D DefaultIcon;
        public static Texture2D HelpIcon;
        public static Texture2D HammerIcon;
        public static Texture2D OldManIcon;
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
                HelpIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Help");
                HammerIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Hammer");
                OldManIcon = ModContent.GetTexture("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Old_Man");
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
            On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
        }

        // 通过调整screenWidth使一切绘制到屏幕之外，NPC对话机制不会被影响
        private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
            // 确保是处于NPC对话状态（PC版中编辑告示牌什么的也是这个UI）
            MobileChat.GUIDrawInner();
        }
    }
}