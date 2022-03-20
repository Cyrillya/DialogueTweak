namespace DialogueTweak
{
    // 一个贴图库，使用贴图时可以直接从这里调用
    internal class HandleAssets : ModSystem
    {

        // 覆盖Icon的List，有一些默认值
        internal static List<IconInfo> IconInfos = new List<IconInfo>() {
            new IconInfo(IconType.Shop, new List<int> { NPCID.Guide }, "DialogueTweak/Interfaces/Assets/Icon_Help"),
            new IconInfo(IconType.Extra, new List<int> { NPCID.Guide }, "DialogueTweak/Interfaces/Assets/Icon_Hammer"),
            new IconInfo(IconType.Shop, new List<int> { NPCID.OldMan }, "DialogueTweak/Interfaces/Assets/Icon_Old_Man"),
            new IconInfo(IconType.Shop, new List<int> { NPCID.Nurse, NPCID.Angler, NPCID.TaxCollector }, "Head")
        };
        internal static Asset<Texture2D> DefaultIcon;
        internal static Asset<Texture2D> SignIcon;
        internal static Asset<Texture2D> EditIcon;
        public override void PostSetupContent() {
            base.PostSetupContent();
            if (Main.netMode != NetmodeID.Server) {
                ButtonHandler.ButtonPanel = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanel");
                ButtonHandler.ButtonPanel_Highlight = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");
                ButtonHandler.Button_Back = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Button_Back");
                ButtonHandler.Button_Happiness = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Button_Happiness");

                ButtonHandler.Shop = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Icon_Default");
                ButtonHandler.Extra = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Icon_Default");

                DefaultIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Icon_Default");
                SignIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Icon_Sign");
                EditIcon = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/Icon_Edit");

                GUIChatDraw.GreyPixel = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/GreyPixel");
                GUIChatDraw.PortraitPanel = ModContent.Request<Texture2D>("DialogueTweak/Interfaces/Assets/PortraitPanel");
                GUIChatDraw.ChatTextPanel = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
            }
        }
    }
}
