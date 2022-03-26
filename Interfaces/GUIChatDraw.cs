using ReLogic.Localization.IME;
using ReLogic.OS;

namespace DialogueTweak.Interfaces
{
    public class GUIChatDraw
    {
        private static TextDisplayCache _textDisplayCache = new TextDisplayCache();

        public static Asset<Texture2D> PortraitPanel;
        public static Vector2 PanelPosition => new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100f);

        public static void GUIDrawInner() {
            if (Main.LocalPlayer.talkNPC < 0 && Main.LocalPlayer.sign == -1) {
                Main.npcChatText = "";
                return;
            }

            int panelValue = 230;
            Color panelColor = new(panelValue, panelValue, panelValue, panelValue);
            int textValue = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new(textValue, textValue, textValue, textValue);

            ChatTextDrawer.PrepareCacheWithTextScrolling(ref _textDisplayCache, out string[] textLines, out int amountOfLines);
            if (Main.editSign) {
                ChatTextDrawer.PrepareBlinker(ref Main.instance.textBlinkerCount, ref Main.instance.textBlinkerState);
                // 输入法面板，不过1.4tml好像不调用游戏内输入法面板了
                Main.instance.DrawWindowsIMEPanel(new Vector2(Main.screenWidth / 2, 90f), 0.5f);
            }
            ChatTextDrawer.PrepareVirtualKeyboard(amountOfLines);
            ChatTextDrawer.PrepareLinesFocuses(ref amountOfLines, out string focusText, out string focusText2, out int money, out float linePositioning);
            ChatTextDrawer.DrawPanel(PanelPosition, linePositioning, panelColor, out Rectangle rectangle);
            var textPanelPosition = PanelPosition + new Vector2(120, 45f);
            var textPanelSize = new Vector2(370f, amountOfLines * 30);
            ChatTextDrawer.DrawTextAndPanel(textPanelPosition, textPanelSize, textColor, _textDisplayCache.TextAppeared, amountOfLines, textLines); // 文字框
            ChatTextDrawer.DrawTextPanelExtra(textPanelPosition, textPanelSize, amountOfLines, money, Main.npcChatCornerItem);
            // 人像背景框，以及名字和文本的分割线
            Main.spriteBatch.Draw(PortraitPanel.Value, PanelPosition + new Vector2(0, 15f), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 肖像
            PortraitDrawer.DrawPortrait(Main.spriteBatch, textColor, rectangle);
            ChatTextDrawer.DrawButtons(PanelPosition, focusText, focusText2, linePositioning);

            // 判断鼠标是否处于交互界面
            if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY))) {
                Main.LocalPlayer.mouseInterface = true;
                // 原版有这么一段，是为了防止两帧都判定了按下
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.mouseLeftRelease = false;
                }
            }
        }
    }
}
