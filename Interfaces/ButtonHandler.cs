namespace DialogueTweak.Interfaces
{
    internal class ButtonHandler
    {
        private static SpriteBatch SpriteBatch => Main.spriteBatch;
        private static int MouseX => Main.mouseX;
        private static int MouseY => Main.mouseY;
        private static int ScreenWidth => Main.screenWidth;

        public static Asset<Texture2D> ButtonPanel;
        public static Asset<Texture2D> ButtonPanel_Highlight;

        public static Asset<Texture2D> Shop;
        public static Rectangle ShopFrame;
        public static Asset<Texture2D> Extra;
        public static Rectangle ExtraFrame;

        public static readonly Color BorderColor = Color.Black;
        public static readonly Color BackgroundColor = new Color(73, 85, 186);
        public static readonly Color HighlightColor = new Color(255, 231, 69);
        public static readonly Color HighlightCornerColor = new Color(233, 176, 0);

        private static bool moveOnBackButton;
        private static bool moveOnHappinessButton;
        private static bool moveOnShopButton;
        private static bool moveOnExtraButton;

        public static void DrawButtons(int statY, string focusText, string focusText2) {
            int talk = Main.LocalPlayer.talkNPC;
            Color textColor = new Color(Main.mouseTextColor, (int)((double)Main.mouseTextColor / 1.1), Main.mouseTextColor / 2, Main.mouseTextColor);
            NPCLoader.SetChatButtons(ref focusText, ref focusText2);

            bool showHappinessReport = Main.LocalPlayer.sign == -1 && Main.LocalPlayer.currentShoppingSettings.HappinessReport != "" && Main.npc[Main.LocalPlayer.talkNPC].townNPC;
            // 返回按钮，小动物由于没有幸福值，所以返回按钮要长一点。由于返回按钮总会显示，就不考虑手柄了
            DrawBackButton(statY, !showHappinessReport);
            if (moveOnBackButton && Main.mouseLeft && Main.mouseLeftRelease) return; // 按下返回按钮后应该停止绘制了，防止数组超限
            if (showHappinessReport) {
                DrawHappinessButton(statY);
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = false; // 考虑手柄

            // 该NPC所有可用的额外按钮
            int type = Main.LocalPlayer.sign != -1 ? -1 : Main.npc[talk].type; // 为了标牌特判
            bool useShopButton = !string.IsNullOrWhiteSpace(focusText);
            bool useExtraButton = !string.IsNullOrWhiteSpace(focusText2);
            List<int> buttons = new(); // 所有该NPC可用额外按钮的index，直接调用HandleAssets.ButtonInfos里的值
            foreach (int i in from a in HandleAssets.ButtonInfos where a.npcTypes.Contains(type) && a.available() select HandleAssets.ButtonInfos.IndexOf(a))
                buttons.Add(i);
            int buttonCounts = buttons.Count + useShopButton.ToInt() + useExtraButton.ToInt();
            if (buttonCounts == 0) return;
            int spacing = 10; // 按扭之间的间隔
            int buttonWidth = 375 / buttonCounts - spacing; // 每个按钮的宽度，+2是加上，-10是去除了按钮之间的间隔

            // 决定图标
            ChatMethods.HandleButtonIcon(type, out Shop, out ShopFrame, ref Extra, ref ExtraFrame);

            int offsetX = 0;

            if (useExtraButton) {
                DrawExtraButton(statY, offsetX, buttonWidth, focusText2, textColor);
                offsetX += buttonWidth + spacing;
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false; // 考虑手柄

            if (useShopButton) {
                DrawShopButton(statY, offsetX, buttonWidth, focusText, textColor);
                offsetX += buttonWidth + spacing;
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = false; // 考虑手柄

            foreach (int i in buttons) {
                DrawModCallButton(i, statY, offsetX, buttonWidth, textColor);
                offsetX += buttonWidth + spacing;
            }
        }

        private static void DrawBackButton(float statY, bool longer) {
            Rectangle buttonRectangle = new Rectangle((int)ChatUI.PanelPosition.X + 16, (int)statY + 10, longer ? 98 : 44, 44);
            var value = ModAsset.Button_Back.Value;
            Rectangle frame = value.Frame();
            // ModCall
            int type = Main.LocalPlayer.sign != -1 ? -1 : Main.npc[Main.LocalPlayer.talkNPC].type; // 为了标牌特判
            foreach (var info in from a in HandleAssets.IconInfos where a.npcTypes.Contains(type) && a.available() && a.texture != "" && a.iconType == IconType.Back select a) {
                value = ModContent.Request<Texture2D>(info.texture).Value;
                frame = info.frame?.Invoke() ?? value.Frame();
            }

            DrawPanel(SpriteBatch, ButtonPanel.Value, buttonRectangle.Location.ToVector2(), buttonRectangle.Size(), Color.White);
            SpriteBatch.Draw(value, buttonRectangle.Location.ToVector2() + buttonRectangle.Size() / 2f, frame, Color.White * 0.9f, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnBackButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnBackButton = true;
                }
                DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, buttonRectangle.Location.ToVector2(), buttonRectangle.Size(), Color.White);
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
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat0, buttonRectangle.Location.ToVector2() + buttonRectangle.Size() / 2f);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsLeft = true;
        }

        private static void DrawHappinessButton(float statY) {
            Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 68, statY + 10);
            var value = ModAsset.Button_Happiness.Value;
            Rectangle frame = value.Frame();
            // ModCall
            int type = Main.LocalPlayer.sign != -1 ? -1 : Main.npc[Main.LocalPlayer.talkNPC].type; // 为了标牌特判
            foreach (var info in from a in HandleAssets.IconInfos where a.npcTypes.Contains(type) && a.available() && a.texture != "" && a.iconType == IconType.Happiness select a) {
                value = ModContent.Request<Texture2D>(info.texture).Value;
                frame = info.frame?.Invoke() ?? value.Frame();
            }

            DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(44, 44), Color.White);
            SpriteBatch.Draw(value, pos, frame, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, ModAsset.Button_Happiness.Width(), ModAsset.Button_Happiness.Height());
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnHappinessButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnHappinessButton = true;
                }
                DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(44, 44), Color.White);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.npcChatCornerItem = 0;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatText = Main.LocalPlayer.currentShoppingSettings.HappinessReport;

                    if (Configuration.Instance.DisplayPreference) {
                        // 点击按钮后在左下角显示具体偏好情况
                        var npc = Main.npc[Main.LocalPlayer.talkNPC];
                        npc.GetNPCPreferenceSorted(out var NPCPreferences, out var biomePreferences);
                        Main.NewText($"[c/{Main.DiscoColor.Hex3()}:{NPC.GetFullnameByID(npc.type)}]");
                        foreach (var preference in NPCPreferences) {
                            Main.NewText(
                                $"{Language.GetTextValue($"Mods.{DialogueTweak.Instance.Name}.{preference.Level}")}: {NPC.GetFullnameByID(preference.NpcId)}");
                        }

                        foreach (var biomes in biomePreferences) {
                            foreach (var biome in biomes.Preferences) {
                                var name = ShopHelper.BiomeNameByKey(biome.Biome.NameKey);
                                // 对于模组群系，直接获取DisplayName的翻译
                                if (biome.Biome is ModBiome modBiome) {
                                    name = modBiome.DisplayName.Value;
                                }

                                Main.NewText(
                                    $"{Language.GetTextValue($"Mods.{DialogueTweak.Instance.Name}.{biome.Affection}")}: {name}");
                            }
                        }
                    }
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

        private static void DrawShopButton(int statY, int offsetX, int width, string shopText, Color chatColor) {
            Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 122 + offsetX, statY + 10);
            int height = 44;
            // 按钮
            DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(width, height), Color.White);
            
            // 对应图像（即icon）
            SpriteBatch.Draw(Shop.Value, pos + new Vector2(44, height) / 2f, ShopFrame, Color.White * 0.9f, 0f, ShopFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnShopButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnShopButton = true;
                }
                DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(width, height), Color.White);
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
            float scale = DecideTextScale(shopText, value, buttonRectangle.Width - ShopFrame.Width - 16f); // 减少值是为了给icon腾出空间
            Color shadowColor = !moveOnShopButton ? Color.Black : Color.Brown;
            Vector2 buttonOffset = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
            
            DrawButtonText(shopText, moveOnShopButton ? 2 : 1.5f, value, buttonOffset, shadowColor, chatColor, scale, pos, out var drawCenter);
            if (scale <= 0.7f && moveOnShopButton) { // 缩放程度太高的放在上面时会在面板下方显示文本
                Vector2 bottom = new Vector2(ScreenWidth / 2, statY + height + 30);
                DrawButtonText(shopText, 1.2f, FontAssets.MouseText.Value, Vector2.Zero, Color.Black, chatColor, 1f, bottom, out _);
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

        private static void DrawExtraButton(int statY, int offsetX, int width, string shopText, Color chatColor) {
            Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 122 + offsetX, statY + 10);
            int height = 44;
            // 按钮
            DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(width, height), Color.White);
            // 对应图像（即icon）
            SpriteBatch.Draw(Extra.Value, pos + new Vector2(44, height) / 2f, ExtraFrame, Color.White * 0.9f, 0f, ExtraFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
                if (!moveOnExtraButton) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    moveOnExtraButton = true;
                }
                // 高光边框
                DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(width, height), Color.White);
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
            float scale = DecideTextScale(shopText, value, buttonRectangle.Width - ExtraFrame.Width - 16f); // 减少值是为了给icon腾出空间
            Color shadowColor = !moveOnExtraButton ? Color.Black : Color.Brown;
            Vector2 buttonOffset = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;

            DrawButtonText(shopText, moveOnExtraButton ? 2 : 1.5f, value, buttonOffset, shadowColor, chatColor, scale, pos, out var drawCenter);
            if (scale <= 0.7f && moveOnExtraButton) { // 缩放程度太高的放在上面时会在面板下方显示文本
                Vector2 bottom = new Vector2(ScreenWidth / 2, statY + height + 30);
                DrawButtonText(shopText, 1.2f, FontAssets.MouseText.Value, Vector2.Zero, Color.Black, chatColor, 1f, bottom, out _);
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

        private static void DrawModCallButton(int i, int statY, int offsetX, int width, Color chatColor) {
            var button = HandleAssets.ButtonInfos[i];
            bool useText = button.buttonText is not null && button.buttonText().Trim() != string.Empty; // 确实有文本
            bool useIcon = button.iconTexture != "";
            string text = !useText ? "" : button.buttonText().Trim();
            Asset<Texture2D> iconTexture = null;

            Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 122 + offsetX, statY + 10);
            int height = 44;
            // 按钮
            DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(width, height), Color.White);
            // 对应图像（即icon）
            if (useIcon) {
                iconTexture = ModContent.Request<Texture2D>(button.iconTexture, AssetRequestMode.ImmediateLoad);
                var iconOffset = new Vector2(!useText ? width : 44, height) / 2f;
                SpriteBatch.Draw(iconTexture.Value, pos + iconOffset, null, Color.White * 0.9f, 0f, new Vector2(iconTexture.Width(), iconTexture.Height()) / 2f, 1f, SpriteEffects.None, 0f);
            }
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
                if (!button.focused) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    button.focused = true;
                }
                // 高光边框
                DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(width, height), Color.White);
                Main.LocalPlayer.mouseInterface = true;

                // 悬停行为
                button.hoverAction.Invoke();
            }
            else if (button.focused) {
                button.focused = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            if (useText) {
                // 还有一个文字提示
                DynamicSpriteFont value = FontAssets.MouseText.Value;
                float scale = DecideTextScale(text, value, buttonRectangle.Width - (useIcon ? iconTexture.Width() + 16f : 0)); // 减少值是为了给icon腾出空间
                Color shadowColor = !button.focused ? Color.Black : Color.Brown;
                Vector2 buttonOrigin = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
                
                DrawButtonText(text, button.focused ? 2 : 1.5f, value, buttonOrigin, shadowColor, chatColor, scale, pos, out _);
                if (scale <= 0.7f && button.focused) { // 缩放程度太高的放在上面时会在面板下方显示文本
                    Vector2 bottom = new Vector2(ScreenWidth / 2, statY + height + 30);
                    DrawButtonText(text, 1.2f, FontAssets.MouseText.Value, Vector2.Zero, Color.Black, chatColor, 1f, bottom, out _);
                }
            }
        }

        private static float DecideTextScale(string text, DynamicSpriteFont font, float maxWidth) {
            Vector2 stringSize = ChatManager.GetStringSize(font, text, Vector2.One); // 先计算出一般情况下(即scale为1)的大小
            if (stringSize.X <= maxWidth) return 1f; // 能容纳的直接给过
            return Math.Max(1f * (maxWidth / stringSize.X), 0.3f); // 不能容纳的进行缩放，最小不能超过0.3
        }

        private static void DrawButtonText(string text, float spread, DynamicSpriteFont font, Vector2 buttonOffset, Color shadowColor, Color chatColor, float sizeScale, Vector2 basePos, out Vector2 pos) {
            var scale = new Vector2(sizeScale, 1f);
            var stringSize = ChatManager.GetStringSize(font, text, scale); // 获取文本真正大小，只进行X轴上的缩放
            float factor = Utils.GetLerpValue(0.6f, 1f, sizeScale, true);
            var offset = new Vector2(MathHelper.Lerp(0f, 6f, factor), 4f); // 根据文本长度调整位置，sizeScale为[0.3-1]的值
            if (sizeScale >= 0.9f && stringSize.X >= 90f) offset.X = 12f; // 给不需要缩放但比较长的文本向右调整，以远离icon
            pos = basePos + buttonOffset + offset;
            
            var array = ChatManager.ParseMessage(text, chatColor).ToArray();
            ChatManager.ConvertNormalSnippets(array);
            ChatManager.DrawColorCodedStringShadow(SpriteBatch, font, array, pos, shadowColor, 0f, stringSize * 0.5f, scale, -1, spread);
            ChatManager.DrawColorCodedString(SpriteBatch, font, array, pos, chatColor, 0f, stringSize * 0.5f, scale, out int _, -1);
        }

        public static void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 size, Color color, Color? cornerColor = null, int cornerSize = 6, int barSize = 32) {
            Color corner = cornerColor ?? color;
            Point point = new Point((int)position.X, (int)position.Y);
            Point point2 = new Point(point.X + (int)size.X - cornerSize, point.Y + (int)size.Y - cornerSize);
            int width = point2.X - point.X - cornerSize;
            int height = point2.Y - point.Y - cornerSize;
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), corner); // left-top corner
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), corner); // right-top corner
            spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, cornerSize, cornerSize), new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), corner); // left-bottom corner
            spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), corner); // right-bottom corner
            spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize), new Rectangle(cornerSize, 0, barSize, cornerSize), color); // top bar
            spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize), new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color); // bottom bar
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, barSize), color); // left bar
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height), new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color); // right bar
            spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, barSize, barSize), color); // middle bar
        }
    }
}
