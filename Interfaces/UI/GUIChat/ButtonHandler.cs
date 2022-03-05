namespace DialogueTweak.Interfaces.UI.GUIChat
{
    internal class ButtonHandler
    {
        private static SpriteBatch SpriteBatch => Main.spriteBatch;
        private static int MouseX => Main.mouseX;
        private static int MouseY => Main.mouseY;
        private static int ScreenWidth => Main.screenWidth;

        public static Asset<Texture2D> Button_Back;
        public static Asset<Texture2D> Button_BackLong;
        public static Asset<Texture2D> Button_Happiness;
        public static Asset<Texture2D> Button_Highlight;

        public static Asset<Texture2D> ButtonLong;
        public static Asset<Texture2D> ButtonLong_Highlight;
        public static Asset<Texture2D> ButtonLonger;
        public static Asset<Texture2D> ButtonLonger_Highlight;

        public static Asset<Texture2D> Shop;
        public static Asset<Texture2D> Extra;

        private static bool moveOnBackButton;
        private static bool moveOnHappinessButton;
        private static bool moveOnShopButton;
        private static bool moveOnExtraButton;

        public static void DrawButtons(float statY) {
            string focusText = "";
            string focusText2 = "";
            int money = 0;
            int num = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new Color(num, num, num, num);
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref textColor, ref money);
            NPCLoader.SetChatButtons(ref focusText, ref focusText2);

            bool showHappinessReport = Main.LocalPlayer.sign == -1 && Main.LocalPlayer.currentShoppingSettings.HappinessReport != "" && Main.npc[Main.LocalPlayer.talkNPC].townNPC;
            // 返回按钮，小动物由于没有幸福值，所以返回按钮要长一点。由于返回按钮总会显示，就不考虑手柄了
            DrawBackButton(statY, !showHappinessReport);
            if (showHappinessReport) {
                DrawHappinessButton(statY);
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = false; // 考虑手柄
            ChatMethods.HandleShopTexture(Main.LocalPlayer.sign != -1 ? -1 : Main.LocalPlayer.talkNPC, ref Shop, ref Extra);
            if (!string.IsNullOrWhiteSpace(focusText)) {
                DrawLongShopButton(statY, focusText, textColor, string.IsNullOrWhiteSpace(focusText2));
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = false; // 考虑手柄
            if (!string.IsNullOrWhiteSpace(focusText2)) {
                DrawLongExtraButton(statY, focusText2, textColor);
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false; // 考虑手柄
        }

        private static void DrawBackButton(float statY, bool longer) {
            Vector2 pos = new Vector2(ScreenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + 16, statY + 10);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, Button_Back.Width(), Button_Back.Height());
            if (longer) {
                buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, Button_BackLong.Width(), Button_BackLong.Height() / 2);
                SpriteBatch.Draw(Button_BackLong.Value, pos, new Rectangle(0, 0, Button_BackLong.Width(), Button_BackLong.Height() / 2), Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else {
                SpriteBatch.Draw(Button_Back.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnBackButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnBackButton = true;
                }
                if (longer) {
                    SpriteBatch.Draw(Button_BackLong.Value, pos, new Rectangle(0, 44, Button_BackLong.Width(), Button_BackLong.Height() / 2), Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                else {
                    SpriteBatch.Draw(Button_Highlight.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.CloseNPCChatOrSign();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
            }
            else if (moveOnBackButton) {
                moveOnBackButton = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            // 手柄支持，这个是最左边
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat0, pos + buttonRectangle.Size() / 2f);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsLeft = true;
        }

        private static void DrawHappinessButton(float statY) {
            Vector2 pos = new Vector2(ScreenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + Button_Back.Width() + 24, statY + 10);
            SpriteBatch.Draw(Button_Happiness.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, Button_Happiness.Width(), Button_Happiness.Height());
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnHappinessButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnHappinessButton = true;
                }
                SpriteBatch.Draw(Button_Highlight.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.npcChatCornerItem = 0;
                    SoundEngine.PlaySound(12);
                    Main.npcChatText = Main.LocalPlayer.currentShoppingSettings.HappinessReport;
                }
            }
            else if (moveOnHappinessButton) {
                moveOnHappinessButton = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            // 手柄支持，这个是左中
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos + buttonRectangle.Size() / 2f);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
        }

        private static void DrawLongShopButton(float statY, string shopText, Color chatColor, bool useLonger) {
            Asset<Texture2D> asset = ButtonLong;
            Asset<Texture2D> highlightAsset = ButtonLong_Highlight;
            Vector2 pos = new Vector2(ScreenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + Button_Back.Width() * 2 + asset.Width() + 48, statY + 10);
            if (useLonger) {
                asset = ButtonLonger;
                highlightAsset = ButtonLonger_Highlight;
                pos = new Vector2(ScreenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + Button_Back.Width() * 2 + 34, statY + 10);
            }
            // 按钮
            SpriteBatch.Draw(asset.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 对应图像
            SpriteBatch.Draw(Shop.Value, pos + new Vector2(asset.Height(), asset.Height()) / 2f, null, Color.White * 0.9f, 0f, new Vector2(Shop.Width(), Shop.Height()) / 2f, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, asset.Width(), asset.Height());
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnShopButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnShopButton = true;
                }
                SpriteBatch.Draw(highlightAsset.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    if (Main.LocalPlayer.sign == -1)
                        ChatMethods.HandleShop(Main.npc[Main.LocalPlayer.talkNPC]);
                    else {
                        if (Main.editSign)
                            Main.SubmitSignText();
                        else
                            IngameFancyUI.OpenVirtualKeyboard(1);
                    }
                }
            }
            else if (moveOnShopButton) {
                moveOnShopButton = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            // 还有一个文字提示
            shopText = shopText.Trim();
            DynamicSpriteFont value = FontAssets.MouseText.Value;
            float scale = DecideTextScale(shopText, value, buttonRectangle.Width - 50); // 减少值是为了给icon腾出空间
            Color shadowColor = (!moveOnShopButton) ? Color.Black : Color.Brown;
            Vector2 buttonOrigin = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
            DrawButtonText(shopText, moveOnShopButton ? 2 : 1.5f, value, buttonOrigin, shadowColor, chatColor, scale, pos, out Vector2 drawCenter);
            if (scale <= 0.7f && moveOnShopButton) { // 缩放程度太高的放在上面时会在面板下方显示文本
                Vector2 bottom = new Vector2(ScreenWidth / 2, statY + asset.Height() + 30);
                DrawButtonText(shopText, 1.5f, FontAssets.MouseText.Value, Vector2.Zero, Color.Black, chatColor, 1f, bottom, out _);
            }

            // 手柄支持，这个是最右边
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat3, drawCenter);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, drawCenter);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = false;
            }
        }
        private static void DrawLongExtraButton(float statY, string shopText, Color chatColor) {
            Asset<Texture2D> asset = ButtonLong;
            Asset<Texture2D> highlightAsset = ButtonLong_Highlight;
            Vector2 pos = new Vector2(ScreenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + Button_Back.Width() * 2 + 34, statY + 10);
            // 按钮
            SpriteBatch.Draw(asset.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 对应图像
            SpriteBatch.Draw(Extra.Value, pos + new Vector2(asset.Height(), asset.Height()) / 2f, null, Color.White * 0.9f, 0f, new Vector2(Extra.Width(), Extra.Height()) / 2f, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, asset.Width(), asset.Height());
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
                if (!moveOnExtraButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnExtraButton = true;
                }
                SpriteBatch.Draw(highlightAsset.Value, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    ChatMethods.HandleExtraButtonClicled(talkNPC);
                }
            }
            else if (moveOnExtraButton) {
                moveOnExtraButton = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            // 还有一个文字提示
            shopText = shopText.Trim();
            DynamicSpriteFont value = FontAssets.MouseText.Value;
            float scale = DecideTextScale(shopText, value, buttonRectangle.Width - 50); // 减少值是为了给icon腾出空间
            Color shadowColor = (!moveOnExtraButton) ? Color.Black : Color.Brown;
            Vector2 buttonOrigin = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
            DrawButtonText(shopText, moveOnExtraButton ? 2 : 1.5f, value, buttonOrigin, shadowColor, chatColor, scale, pos, out Vector2 drawCenter);
            if (scale <= 0.7f && moveOnExtraButton) { // 缩放程度太高的放在上面时会在面板下方显示文本
                Vector2 bottom = new Vector2(ScreenWidth / 2, statY + asset.Height() + 30);
                DrawButtonText(shopText, 1.5f, FontAssets.MouseText.Value, Vector2.Zero, Color.Black, chatColor, 1f, bottom, out _);
            }

            // 手柄支持，这个是右中
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat2, drawCenter);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, drawCenter);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false;
            }
        }

        private static float DecideTextScale(string text, DynamicSpriteFont font, float maxWidth) {
            Vector2 stringSize = ChatManager.GetStringSize(font, text, Vector2.One); // 先计算出一般情况下(即scale为1)的大小
            if (stringSize.X <= maxWidth) return 1f; // 能容纳的直接给过
            return Math.Max(1f * (maxWidth / stringSize.X), 0.5f); // 不能容纳的进行缩放，最小不能超过0.5
        }

        private static void DrawButtonText(string text, float spread, DynamicSpriteFont font, Vector2 buttonOrigin, Color shadowColor, Color chatColor, float sizeScale, Vector2 basePos, out Vector2 pos) {
            var scale = new Vector2(sizeScale, 1f);
            var stringSize = ChatManager.GetStringSize(font, text, scale); // 获取文本真正大小，只进行X轴上的缩放
            Vector2 offset = new Vector2(MathHelper.Lerp(-12f, 4f, sizeScale), 4f); // 根据文本长度调整位置，根据缩放的大小可以让文本往左靠一点，尽量避免脱离按钮，sizeScale为[0.5-1]的值
            if (sizeScale >= 0.9f && stringSize.X >= 90f) offset.X = 12f; // 给不需要缩放但比较长的文本向右调整，以远离icon
            pos = basePos + buttonOrigin + offset;
            ChatManager.DrawColorCodedStringShadow(SpriteBatch, font, text, pos, shadowColor, 0f, stringSize * 0.5f, scale, -1, spread);
            ChatManager.DrawColorCodedString(SpriteBatch, font, text, pos, chatColor, 0f, stringSize * 0.5f, scale);
        }
    }
}
