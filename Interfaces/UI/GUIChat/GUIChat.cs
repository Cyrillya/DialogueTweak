using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace DialogueTweak.Interfaces.UI.GUIChat
{
    public class GUIChat
    {
        SpriteBatch spriteBatch => Main.spriteBatch;
        int ScreenWidth => Main.screenWidth;
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
                    TextLines = ChatMethods.WordwrapString(_originalText, Main.fontMouseText, 365, 27, out int lines);
                    AmountOfLines = lines;
                    TextAppeared = 0;
                }
            }
        }
        private TextDisplayCache _textDisplayCache = new TextDisplayCache();
        public int textBlinkerCount { get => Main.instance.textBlinkerCount; set => Main.instance.textBlinkerCount = value; }
        public int textBlinkerState { get => Main.instance.textBlinkerState; set => Main.instance.textBlinkerState = value; }

        public static Texture2D GreyPixel;
        public static Texture2D PortraitPanel;
        public static Texture2D ChatStringBack;
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
            var textLines = _textDisplayCache.TextLines;
            int amountOfLines = _textDisplayCache.AmountOfLines;

            // 用于标牌 | 竖线闪动机制
            if (Main.editSign) {
                textBlinkerCount++;
                if (textBlinkerCount >= 20) {
                    if (textBlinkerState == 0)
                        textBlinkerState = 1;
                    else
                        textBlinkerState = 0;

                    textBlinkerCount = 0;
                }

                // 输入法面板
                Main.instance.DrawWindowsIMEPanel(new Vector2(ScreenWidth / 2, 90f), 0.5f);
            }

            // 手柄虚拟键盘的位置，很帅哦
            int num2 = 120 + amountOfLines * 30 + 30;
            num2 -= 180;
            if (!PlayerInput.UsingGamepad)
                num2 = 9999;
            UIVirtualKeyboard.OffsetDown = num2;

            amountOfLines++;
            if (Main.LocalPlayer.sign > -1) _textDisplayCache.TextAppeared = 1145141919; // 标牌没有缓慢出现机制
            else _textDisplayCache.TextAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * (GameCulture.Chinese.IsActive ? 1 : 1.8f);
            // 判断一下：如果有钱币显示（税收官，护士之类的）则多加一行
            string focusText = "";
            string focusText2 = "";
            Color c = default;
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref c, out int money);
            if (money != 0) {
                amountOfLines++;
            }

            // 开始Draw了
            float linePositioning = (amountOfLines < 2 ? 2 : amountOfLines) + 3; // float更方便细调，其实就是为了手柄编辑标牌的对称感
            if (Main.editSign && PlayerInput.UsingGamepad) { // 手柄下编辑标牌时底部没有按钮，会缩回去
                linePositioning -= 1.85f;
            }
            // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接用最大行数限制吧
            spriteBatch.Draw(Main.chatBackTexture, new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2, 100f), new Rectangle(0, 0, Main.chatBackTexture.Width, (int)(linePositioning >= 15 ? 450 : 2 + linePositioning * 30)), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            if (linePositioning >= 15) spriteBatch.Draw(Main.chatBackTexture, new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2, 550f), new Rectangle(0, 30, Main.chatBackTexture.Width, (int)(2 + (linePositioning - 15) * 30)), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.chatBackTexture, new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2, 102 + linePositioning * 30), new Rectangle(0, Main.chatBackTexture.Height - 30, Main.chatBackTexture.Width, 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            // 碰撞箱
            var rectangle = new Rectangle(ScreenWidth / 2 - Main.chatBackTexture.Width / 2, 100, Main.chatBackTexture.Width, (int)(2 + linePositioning * 30 + 30));

            // 文字，还有一个方框深黑底
            if (amountOfLines > 1) {
                spriteBatch.Draw(ChatStringBack, new Vector2(268 + (ScreenWidth - 800) / 2, 145f), new Rectangle(0, 0, ChatStringBack.Width, (amountOfLines - 1) * 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ChatStringBack, new Vector2(268 + (ScreenWidth - 800) / 2, 145f + (amountOfLines - 1) * 30), new Rectangle(0, ChatStringBack.Height - 30, ChatStringBack.Width, 30), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            // 只有一行，为了避免黑底显示错误，做一下特判
            else {
                spriteBatch.Draw(ChatStringBack, new Vector2(268 + (ScreenWidth - 800) / 2, 145f), new Rectangle(0, 0, ChatStringBack.Width, 15), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ChatStringBack, new Vector2(268 + (ScreenWidth - 800) / 2, 145f + 15), new Rectangle(0, ChatStringBack.Height - 15, ChatStringBack.Width, 15), panelColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            int shownCharCount = 0;
            // 对话文本
            for (int i = 0; i < amountOfLines; i++) {
                var text = textLines[i];
                if (text != null) {
                    string showText = "";
                    for (int j = 0; j < text.Length; j++) {
                        showText += text[j];
                        shownCharCount++;
                        if (shownCharCount >= _textDisplayCache.TextAppeared) {
                            break;
                        }
                    }
                    var font = Main.fontMouseText;
                    var basePos = new Vector2(273 + (ScreenWidth - 800) / 2, 150 + i * 30);
                    // 输入法缓冲文本与光标闪动
                    if (i == amountOfLines - 1 && Main.editSign) {
                        var drawCursor = basePos + new Vector2(ChatManager.GetStringSize(font, showText, Vector2.One).X, 0);
                        string compositionString = Platform.Current.Ime.CompositionString;
                        if (compositionString != null && compositionString.Length > 0) {
                            Vector2 position = drawCursor;
                            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, compositionString, position, Color.Black, 0f, Vector2.Zero, Vector2.One, spread: 1.2f);
                            Utils.DrawBorderStringFourWay(spriteBatch, font, compositionString, position.X, position.Y, new Color(255, 240, 20), Color.Transparent, Vector2.Zero);
                            drawCursor.X += font.MeasureString(compositionString).X; // the cursor drawing position should be changed.
                        }
                        if (textBlinkerState == 1)
                            Utils.DrawBorderStringFourWay(spriteBatch, font, "|", drawCursor.X, drawCursor.Y, Color.White, Color.Black, Vector2.Zero);
                    }
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, showText, basePos, textColor, 0, Vector2.Zero, Vector2.One);
                    if (shownCharCount >= _textDisplayCache.TextAppeared) {
                        break;
                    }
                }
            }
            if (money != 0) {
                ItemSlot.DrawMoney(spriteBatch, "", ScreenWidth / 2 + 106, 80 + amountOfLines * 30, Utils.CoinsSplit(money), horizontal: true);
            }

            // 任务物品展示
            if (Main.npcChatCornerItem != 0) {
                Vector2 position = new Vector2(ScreenWidth / 2 + Main.chatBackTexture.Width / 2 - 8, 30 + (amountOfLines + 3) * 30 + 30);
                position -= Vector2.One * 8f;
                Item Item = new Item();
                Item.netDefaults(Main.npcChatCornerItem);
                float num3 = 1f;
                Texture2D value = Main.itemTexture[Item.type];
                if (value.Width > 32 || value.Height > 32)
                    num3 = ((value.Width <= value.Height) ? (32f / (float)value.Height) : (32f / (float)value.Width));

                spriteBatch.Draw(value, position, null, Item.GetAlpha(Color.White), 0f, new Vector2(value.Width, value.Height), num3, SpriteEffects.None, 0f);
                if (Item.color != default(Color))
                    spriteBatch.Draw(value, position, null, Item.GetColor(Item.color), 0f, new Vector2(value.Width, value.Height), num3, SpriteEffects.None, 0f);

                if (new Rectangle((int)position.X - (int)((float)value.Width * num3), (int)position.Y - (int)((float)value.Height * num3), (int)((float)value.Width * num3), (int)((float)value.Height * num3)).Contains(new Point(Main.mouseX, Main.mouseY)))
                    Main.instance.MouseText(Item.Name, -11, 0);
            }

            // 人像背景框，以及名字和文本的分割线
            spriteBatch.Draw(PortraitPanel, new Vector2(ScreenWidth / 2 - PortraitPanel.Width / 2, 115f), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if (Main.LocalPlayer.talkNPC >= 0 && Main.LocalPlayer.sign == -1 && Main.npc[Main.LocalPlayer.talkNPC] != null && Main.npc[Main.LocalPlayer.talkNPC].active) {
                var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

                float scale = 1.8f;

                Texture2D value = Main.npcTexture[talkNPC.type];

                var position = new Vector2(ScreenWidth / 2 - PortraitPanel.Width / 2, 115f) + new Vector2(62f, 48f);
                var origin = new Vector2(talkNPC.frame.Width, talkNPC.frame.Height) / 2f;
                if (origin.Y >= 28f) position.Y -= (origin.Y - 28f) * scale; // 手动调整防止超框

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

                spriteBatch.Draw(value, position, talkNPC.frame, talkNPC.GetAlpha(Color.White), 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);
                if (talkNPC.color != default(Color))
                    spriteBatch.Draw(value, position, talkNPC.frame, talkNPC.GetColor(talkNPC.color), 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);

                // 名字，用DeathText因为它大而且清晰
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontDeathText, talkNPC.GivenOrTypeName, 270f + (ScreenWidth - 800) / 2, 106, textColor, Color.Black, Vector2.Zero, 0.6f);

                spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.MountedSamplerState, null, null, null, Main.UIScaleMatrix);
            }

            // 标牌
            if (Main.LocalPlayer.sign > -1) {
                // 画像
                var value = DialogueTweak.SignIcon;
                int i = Main.LocalPlayer.sign;
                if (Main.sign[i] is null || !WorldGen.InWorld(Main.sign[i].x, Main.sign[i].y) || !Main.tile[Main.sign[i].x, Main.sign[i].y].active())
                    return;

                // 名字与图像
                var tile = Main.tile[Main.sign[i].x, Main.sign[i].y];
                string text = Lang._mapLegendCache.FromTile(Main.Map[Main.sign[i].x, Main.sign[i].y], Main.sign[i].x, Main.sign[i].y);
                ChatMethods.GetSignItemType(Main.sign[i].x, Main.sign[i].y, tile.type, out int dropItem); // 获取物品
                if (dropItem < Main.itemTexture.Length && Main.itemTexture[dropItem] != null) {
                    value = Main.itemTexture[dropItem];
                    var item = new Item();
                    item.netDefaults(dropItem);
                    text = item.Name;
                }

                var position = new Vector2(ScreenWidth / 2 - PortraitPanel.Width / 2, 115f) + new Vector2(62f, 50f);
                // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

                spriteBatch.Draw(value, position, null, Color.White, 0f, value.Size() / 2f, 2f, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontDeathText, text, 270f + (ScreenWidth - 800) / 2, 106, textColor, Color.Black, Vector2.Zero, 0.6f);

                spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.MountedSamplerState, null, null, null, Main.UIScaleMatrix);
            }

            // 在交互按钮和对话之间一条浅黑线，如果在手柄状态下编辑文本的话没有交互按钮，且UI底部也会缩回去，就不Draw了
            if (!Main.editSign || !PlayerInput.UsingGamepad) {
                byte breakPixel = 2; // 左右都有[breakPixel]个像素的空隙
                spriteBatch.Draw(GreyPixel, new Vector2(breakPixel * 2 + ScreenWidth / 2 - Main.chatBackTexture.Width / 2, linePositioning * 30 + 70), null, Color.White * 0.9f, 0f, Vector2.Zero, new Vector2(Main.chatBackTexture.Width - breakPixel * 4, 3f), SpriteEffects.None, 0f);
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
