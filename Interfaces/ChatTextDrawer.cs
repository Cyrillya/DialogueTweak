using ReLogic.Localization.IME;
using ReLogic.OS;

namespace DialogueTweak.Interfaces
{
    internal static class ChatTextDrawer
    {
        public static Asset<Texture2D> ChatTextPanel;
        public static Asset<Texture2D> GreyPixel;
        public static readonly Color ChatTextPanelColor = new Color(35, 43, 89);

        internal static void DrawButtons(Vector2 panelPosition, string focusText, string focusText2, float linePositioning) {
            bool shouldDrawButton = Main.InGameUI.CurrentState is not UIVirtualKeyboard || !PlayerInput.UsingGamepad;
            if (shouldDrawButton) {
                // 在交互按钮和对话之间一条浅黑线，如果没有交互按钮就不Draw了（绘制条件和交互按钮一致）
                byte breakPixel = 2; // 左右都有[breakPixel]个像素的空隙
                Main.spriteBatch.Draw(GreyPixel.Value, panelPosition + new Vector2(breakPixel * 2, linePositioning * 30 - 30), null, Color.White * 0.9f, 0f, Vector2.Zero, new Vector2(TextureAssets.ChatBack.Width() - breakPixel * 4, 3f), SpriteEffects.None, 0f);
                // 按钮
                ButtonHandler.DrawButtons((int)(linePositioning * 30 - 30 + panelPosition.Y), focusText, focusText2);
            }
        }

        internal static void DrawTextPanelExtra(Vector2 textPanelPosition, Vector2 textPanelSize, int amountOfLines, int money, int itemType) {
            if (money != 0) {
                ItemSlot.DrawMoney(Main.spriteBatch, "", Main.screenWidth / 2 + 106, 80 + amountOfLines * 30, Utils.CoinsSplit(money), horizontal: true);
            }

            // 任务物品展示
            if (itemType != 0) {
                Vector2 position = textPanelPosition + textPanelSize;
                position -= Vector2.One * 4f;
                Item Item = new Item();
                Item.netDefaults(itemType);
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
        }

        internal static void DrawTextAndPanel(Vector2 textPanelPosition, Vector2 textPanelSize, Color textColor, float textAppeared, int amountOfLines, string[] textLines) {
            ButtonHandler.DrawPanel(Main.spriteBatch, ChatTextPanel.Value,
                textPanelPosition, // position
                textPanelSize, // size
                ChatTextPanelColor, cornerSize: 12, barSize: 4);

            // 对话文本
            int shownCharCount = 0;
            for (int i = 0; i < amountOfLines; i++) {
                string text = textLines[i];
                if (text != null) {
                    string showText = "";
                    for (int j = 0; j < text.Length; j++) {
                        showText += text[j];
                        shownCharCount++;
                        if (shownCharCount >= textAppeared) {
                            break;
                        }
                    }
                    var font = FontAssets.MouseText.Value;
                    var basePos = textPanelPosition + new Vector2(5, 5 + i * 30);
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
                        if (Main.instance.textBlinkerState == 1)
                            Utils.DrawBorderStringFourWay(Main.spriteBatch, font, "|", drawCursor.X, drawCursor.Y, Color.White, Color.Black, Vector2.Zero);
                    }
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, showText, basePos, textColor, 0, Vector2.Zero, Vector2.One);
                    if (shownCharCount >= textAppeared) {
                        break;
                    }
                }
            }
        }

        internal static void DrawPanel(Vector2 position, float linePositioning, Color color, out Rectangle rectangle) {
            // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接用最大行数限制吧
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value, position: position,
                sourceRectangle: new Rectangle(0, 0, TextureAssets.ChatBack.Width(), (int)(linePositioning >= 15 ? 450 : 2 + linePositioning * 30)),
                color: color);
            if (linePositioning >= 15) {
                Main.spriteBatch.Draw(
                    texture: TextureAssets.ChatBack.Value,
                    position: position + new Vector2(0, 450f),
                    sourceRectangle: new Rectangle(0, 30, TextureAssets.ChatBack.Width(), (int)(2 + (linePositioning - 15) * 30)),
                    color: color);
            }
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value, position: position + new Vector2(0, 2 + linePositioning * 30),
                sourceRectangle: new Rectangle(0, TextureAssets.ChatBack.Height() - 30, TextureAssets.ChatBack.Width(), 30),
                color: color);
            rectangle = new Rectangle((int)position.X, (int)position.Y, TextureAssets.ChatBack.Width(), (int)(2 + linePositioning * 30 + 30));
        }

        /// <summary>用于标牌 | 竖线闪动机制</summary>
        internal static void PrepareBlinker(ref int textBlinkerCount, ref int textBlinkerState) {
            textBlinkerCount++;
            if (textBlinkerCount >= 20) {
                if (textBlinkerState == 0)
                    textBlinkerState = 1;
                else
                    textBlinkerState = 0;

                textBlinkerCount = 0;
            }
        }

        /// <summary>手柄虚拟键盘的位置</summary>
        internal static void PrepareVirtualKeyboard(int amountOfLines) {
            int num2 = 120 + amountOfLines * 30 + 30;
            num2 -= 180;
            UIVirtualKeyboard.ShouldHideText = !PlayerInput.UsingGamepad;
            if (!PlayerInput.UsingGamepad) // 不使用手柄就隐藏了
                num2 = 9999;
            UIVirtualKeyboard.OffsetDown = num2;
        }

        /// <summary>按钮文本、钱币以及用于绘制的基于行数的定位</summary>
        internal static void PrepareLinesFocuses(ref int amountOfLines, out string focusText, out string focusText2, out int money, out float linePositioning) {
            amountOfLines++;

            // 判断一下：如果有钱币显示（税收官，护士之类的）则多加一行
            focusText = "";
            focusText2 = "";
            money = 0;
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref money);
            if (money != 0) {
                amountOfLines++;
            }

            linePositioning = (amountOfLines < 2 ? 2 : amountOfLines) + 3; // float更方便细调，其实就是为了手柄编辑标牌的对称感
            if (Main.editSign && !UIVirtualKeyboard.ShouldHideText) { // 手柄下编辑标牌时底部没有按钮，会缩回去
                linePositioning -= 1.85f;
            }
        }

        internal static void PrepareCacheWithTextScrolling(ref TextDisplayCache textDisplayCache, out string[] textLines, out int amountOfLines) {
            textDisplayCache.PrepareCache(Main.npcChatText); // 处理对话文本（换行、统计行数之类）
            textLines = textDisplayCache.TextLines;
            amountOfLines = textDisplayCache.AmountOfLines;

            float speakingRateMultipiler = GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive ? 1 : 1.8f;
            if (Main.LocalPlayer.sign > -1) textDisplayCache.TextAppeared = 1145141919; // 标牌没有缓慢出现机制
            else textDisplayCache.TextAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * speakingRateMultipiler;
        }
    }
}
