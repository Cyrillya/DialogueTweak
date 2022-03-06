using ReLogic.Localization.IME;
using ReLogic.OS;

namespace DialogueTweak.Interfaces
{
    public class GUIChatDraw
    {
        protected class TextDisplayCache
        {
            private string _originalText;
            private int _lastScreenWidth;
            private int _lastScreenHeight;

            public string[] TextLines {
                get;
                private set;
            }

            public int AmountOfLines {
                get;
                private set;
            }

            public float TextAppeared;

            public void PrepareCache(string text) {
                if ((0 | ((Main.screenWidth != _lastScreenWidth) ? 1 : 0) | ((Main.screenHeight != _lastScreenHeight) ? 1 : 0) | ((_originalText != text) ? 1 : 0)) != 0) {
                    _lastScreenWidth = Main.screenWidth;
                    _lastScreenHeight = Main.screenHeight;
                    _originalText = text;
                    TextLines = ChatMethods.WordwrapString(Main.npcChatText, FontAssets.MouseText.Value, 365, out int lineAmount);
                    AmountOfLines = lineAmount;
                    TextAppeared = 0;
                }
            }
        }
        private TextDisplayCache _textDisplayCache = new TextDisplayCache();
        public int textBlinkerCount { get => Main.instance.textBlinkerCount; set => Main.instance.textBlinkerCount = value; }
        public int textBlinkerState { get => Main.instance.textBlinkerState; set => Main.instance.textBlinkerState = value; }

        public static Asset<Texture2D> GreyPixel;
        public static Asset<Texture2D> PortraitPanel;
        public static Asset<Texture2D> ChatStringBack;
        public void GUIDrawInner() {
            if (Main.LocalPlayer.talkNPC < 0 && Main.LocalPlayer.sign == -1) {
                Main.npcChatText = "";
                return;
            }

            int panelValue = 230;
            Color panelColor = new Color(panelValue, panelValue, panelValue, panelValue);
            int textValue = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new Color(textValue, textValue, textValue, textValue);
            bool flag = Main.InGameUI.CurrentState is UIVirtualKeyboard && PlayerInput.UsingGamepad;

            _textDisplayCache.PrepareCache(Main.npcChatText); // 处理对话文本（换行、统计行数之类）
            string[] textLines = _textDisplayCache.TextLines;
            int amountOfLines = _textDisplayCache.AmountOfLines;

            // 用于标牌 | 竖线闪动机制，和输入法
            if (Main.editSign) {
                textBlinkerCount++;
                if (textBlinkerCount >= 20) {
                    if (textBlinkerState == 0)
                        textBlinkerState = 1;
                    else
                        textBlinkerState = 0;

                    textBlinkerCount = 0;
                }

                // 输入法面板，不过1.4tml好像不调用游戏内输入法面板了
                Main.instance.DrawWindowsIMEPanel(new Vector2(Main.screenWidth / 2, 90f), 0.5f);
            }

            // 手柄虚拟键盘的位置，很帅哦
            int num2 = 120 + amountOfLines * 30 + 30;
            num2 -= 180;
            UIVirtualKeyboard.ShouldHideText = !PlayerInput.UsingGamepad;
            if (!PlayerInput.UsingGamepad)
                num2 = 9999;
            UIVirtualKeyboard.OffsetDown = num2;

            amountOfLines++;
            if (Main.LocalPlayer.sign > -1) _textDisplayCache.TextAppeared = 1145141919; // 标牌没有缓慢出现机制
            else _textDisplayCache.TextAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * (GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive ? 1 : 1.8f);
            // 判断一下：如果有钱币显示（税收官，护士之类的）则多加一行
            string focusText = "";
            string focusText2 = "";
            int money = 0;
            Color c = default;
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref c, ref money);
            if (money != 0) {
                amountOfLines++;
            }

            // 开始Draw了
            float linePositioning = (amountOfLines < 2 ? 2 : amountOfLines) + 3; // float更方便细调，其实就是为了手柄编辑标牌的对称感
            if (Main.editSign && !UIVirtualKeyboard.ShouldHideText) { // 手柄下编辑标牌时底部没有按钮，会缩回去
                linePositioning -= 1.85f;
            }
            // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接用最大行数限制吧
            Main.spriteBatch.Draw(TextureAssets.ChatBack.Value, new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100f), new Rectangle(0, 0, TextureAssets.ChatBack.Width(), (int)(linePositioning >= 15 ? 450 : 2 + linePositioning * 30)), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            if (linePositioning >= 15) Main.spriteBatch.Draw(TextureAssets.ChatBack.Value, new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 550f), new Rectangle(0, 30, TextureAssets.ChatBack.Width(), (int)(2 + (linePositioning - 15) * 30)), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TextureAssets.ChatBack.Value, new Vector2(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 102 + linePositioning * 30), new Rectangle(0, TextureAssets.ChatBack.Height() - 30, TextureAssets.ChatBack.Width(), 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            // 碰撞箱
            var rectangle = new Rectangle(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100, TextureAssets.ChatBack.Width(), (int)(2 + linePositioning * 30 + 30));

            // 文字，还有一个方框深黑底
            if (amountOfLines > 1) {
                Main.spriteBatch.Draw(ChatStringBack.Value, new Vector2(268 + (Main.screenWidth - 800) / 2, 145f), new Rectangle(0, 0, ChatStringBack.Width(), (amountOfLines - 1) * 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(ChatStringBack.Value, new Vector2(268 + (Main.screenWidth - 800) / 2, 145f + (amountOfLines - 1) * 30), new Rectangle(0, ChatStringBack.Height() - 30, ChatStringBack.Width(), 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            // 只有一行，为了避免黑底显示错误，做一下特判
            else {
                Main.spriteBatch.Draw(ChatStringBack.Value, new Vector2(268 + (Main.screenWidth - 800) / 2, 145f), new Rectangle(0, 0, ChatStringBack.Width(), 15), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(ChatStringBack.Value, new Vector2(268 + (Main.screenWidth - 800) / 2, 145f + 15), new Rectangle(0, ChatStringBack.Height() - 15, ChatStringBack.Width(), 15), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            int shownCharCount = 0;
            // 对话文本
            for (int i = 0; i < amountOfLines; i++) {
                string text = textLines[i];
                if (text != null) {
                    string showText = "";
                    for (int j = 0; j < text.Length; j++) {
                        showText += text[j];
                        shownCharCount++;
                        if (shownCharCount >= _textDisplayCache.TextAppeared) {
                            break;
                        }
                    }
                    var font = FontAssets.MouseText.Value;
                    var basePos = new Vector2(273 + (Main.screenWidth - 800) / 2, 150 + i * 30);
                    // 输入法缓冲文本与光标闪动
                    if (i == amountOfLines - 1 && Main.editSign) {
                        var drawCursor = basePos + new Vector2(ChatManager.GetStringSize(font, showText, Vector2.One).X, 0);
                        string compositionString = Platform.Get<IImeService>().CompositionString;
                        if (compositionString != null && compositionString.Length > 0) {
                            Vector2 position = drawCursor;
                            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, compositionString, position, Color.Black, 0f, Vector2.Zero, Vector2.One, spread: 1.2f);
                            Utils.DrawBorderStringFourWay(Main.spriteBatch, font, compositionString, position.X, position.Y, new Color(255, 240, 20), Color.Transparent, Vector2.Zero);
                            drawCursor.X += font.MeasureString(compositionString).X; // the cursor drawing position should be changed.
                        }
                        if (textBlinkerState == 1)
                            Utils.DrawBorderStringFourWay(Main.spriteBatch, font, "|", drawCursor.X, drawCursor.Y, Color.White, Color.Black, Vector2.Zero);
                    }
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, showText, basePos, textColor, 0, Vector2.Zero, Vector2.One);
                    if (shownCharCount >= _textDisplayCache.TextAppeared) {
                        break;
                    }
                }
            }
            if (money != 0) {
                ItemSlot.DrawMoney(Main.spriteBatch, "", Main.screenWidth / 2 + 106, 80 + amountOfLines * 30, Utils.CoinsSplit(money), horizontal: true);
            }

            // 任务物品展示
            if (Main.npcChatCornerItem != 0) {
                Vector2 position = new Vector2(Main.screenWidth / 2 + TextureAssets.ChatBack.Width() / 2 - 8, 30 + (amountOfLines + 3) * 30 + 30);
                position -= Vector2.One * 8f;
                Item Item = new Item();
                Item.netDefaults(Main.npcChatCornerItem);
                float num3 = 1f;
                Main.instance.LoadItem(Item.type);
                Texture2D value = TextureAssets.Item[Item.type].Value;
                if (value.Width > 32 || value.Height > 32)
                    num3 = ((value.Width <= value.Height) ? (32f / (float)value.Height) : (32f / (float)value.Width));

                Main.spriteBatch.Draw(value, position, null, Item.GetAlpha(Color.White), 0f, new Vector2(value.Width, value.Height), num3, SpriteEffects.None, 0f);
                if (Item.color != default(Color))
                    Main.spriteBatch.Draw(value, position, null, Item.GetColor(Item.color), 0f, new Vector2(value.Width, value.Height), num3, SpriteEffects.None, 0f);

                if (new Rectangle((int)position.X - (int)((float)value.Width * num3), (int)position.Y - (int)((float)value.Height * num3), (int)((float)value.Width * num3), (int)((float)value.Height * num3)).Contains(new Point(Main.mouseX, Main.mouseY)))
                    Main.instance.MouseText(Item.Name, -11, 0);
            }

            // 人像背景框，以及名字和文本的分割线
            Main.spriteBatch.Draw(PortraitPanel.Value, new Vector2(Main.screenWidth / 2 - PortraitPanel.Width() / 2, 115f), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 肖像
            PortraitDrawer.DrawPortrait(Main.spriteBatch, textColor, rectangle);

            // 在交互按钮和对话之间一条浅黑线，如果在手柄状态下编辑文本的话没有交互按钮，且UI底部也会缩回去，就不Draw了
            if (!Main.editSign || UIVirtualKeyboard.ShouldHideText) {
                byte breakPixel = 2; // 左右都有[breakPixel]个像素的空隙
                Main.spriteBatch.Draw(GreyPixel.Value, new Vector2(breakPixel * 2 + Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, linePositioning * 30 + 70), null, Color.White * 0.9f, 0f, Vector2.Zero, new Vector2(TextureAssets.ChatBack.Width() - breakPixel * 4, 3f), SpriteEffects.None, 0f);
            }

            // 交互按钮
            if (!flag) ButtonHandler.DrawButtons(linePositioning * 30 + 70);

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
