using System.Collections.Generic;

namespace DialogueTweak
{
    // 一个贴图库，使用贴图时可以直接从这里调用
    internal class HandleAssets : ModSystem
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
        internal static Asset<Texture2D> DefaultIcon;
        internal static Asset<Texture2D> SignIcon;
        internal static Asset<Texture2D> EditIcon;
        public override void PostSetupContent() {
            base.PostSetupContent();
            if (Main.netMode != NetmodeID.Server) {
                ButtonHandler.Button_Back = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Back");
                ButtonHandler.Button_BackLong = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_BackLong");
                ButtonHandler.Button_Happiness = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Happiness");
                ButtonHandler.Button_Highlight = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Button_Highlight");

                ButtonHandler.ButtonLong = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLong");
                ButtonHandler.ButtonLong_Highlight = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLong_Highlight");
                ButtonHandler.ButtonLonger = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLonger");
                ButtonHandler.ButtonLonger_Highlight = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/ButtonLonger_Highlight");

                ButtonHandler.Shop = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");
                ButtonHandler.Extra = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");

                DefaultIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Default");
                SignIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Sign");
                EditIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/Buttons/Icon_Edit");

                GUIChat.GreyPixel = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/GreyPixel");
                GUIChat.PortraitPanel = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/PortraitPanel");
                GUIChat.ChatStringBack = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/UI/GUIChat/ChatStringBack");
            }
        }
    }
}
