using ReLogic.Localization.IME;
using ReLogic.OS;

namespace DialogueTweak.Interfaces
{
    public class GUIChatDraw
    {
        private static TextDisplayCache _textDisplayCache = new();

        internal static Asset<Texture2D> BiomeIconTags;
        internal static Asset<Texture2D> PortraitPanel;
        internal static Asset<Texture2D> ChatTextPanel;
        internal static Asset<Texture2D> GreyPixel;
        public static readonly Color ChatTextPanelColor = new(35, 43, 89);
        public static readonly Vector2 PanelPosition = new(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100f);

        public static void GUIDrawInner() {
            if (Main.LocalPlayer.talkNPC < 0 && Main.LocalPlayer.sign == -1) {
                Main.npcChatText = "";
                return;
            }

            int panelValue = 230;
            Color panelColor = new(panelValue, panelValue, panelValue, panelValue);
            int textValue = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new(textValue, textValue, textValue, textValue);

            PrepareCache(ref _textDisplayCache, out string[] textLines, out int amountOfLines);
            if (Main.editSign) {
                PrepareBlinker(ref Main.instance.textBlinkerCount, ref Main.instance.textBlinkerState);
                // 输入法面板，不过1.4tml好像不调用游戏内输入法面板了
                Main.instance.DrawWindowsIMEPanel(new Vector2(Main.screenWidth / 2, 90f), 0.5f);
            }
            PrepareVirtualKeyboard(amountOfLines);
            PrepareLinesFocuses(ref amountOfLines, out string focusText, out string focusText2, out int money, out float linePositioning);
            DrawPanel(linePositioning, panelColor, out Rectangle rectangle);
            var textPanelPosition = PanelPosition + new Vector2(120, 45f);
            var textPanelSize = new Vector2(370f, amountOfLines * 30);
            DrawTextAndPanel(textPanelPosition, textPanelSize, textColor, DialogueTweakSystem.letterAppeared, amountOfLines, textLines); // 文字框
            // 人像背景框，以及名字和文本的分割线
            Main.spriteBatch.Draw(PortraitPanel.Value, PanelPosition + new Vector2(0, 15f), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 钱币、物品、偏好之类
            DrawTextPanelExtra(textPanelPosition, textPanelSize, rectangle, money, Main.npcChatCornerItem);
            // 肖像
            PortraitDrawer.DrawPortrait(Main.spriteBatch, textColor, rectangle);
            DrawButtons(focusText, focusText2, linePositioning);

            // 判断鼠标是否处于对话栏界面
            if (new Rectangle((int)textPanelPosition.X, (int)textPanelPosition.Y, (int)textPanelSize.X, (int)textPanelSize.Y).Contains(new Point(Main.mouseX, Main.mouseY))) {
                DialogueTweakSystem.cursorAtTextPanel = true;
            }

            // 判断鼠标是否处于交互界面
            if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY))) {
                Main.LocalPlayer.mouseInterface = true;
                // 原版有这么一段，是为了防止两帧都判定了按下
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.mouseLeftRelease = false;
                }
            }
        }

        internal static void DrawButtons(string focusText, string focusText2, float linePositioning) {
            bool shouldDrawButton = Main.InGameUI.CurrentState is not UIVirtualKeyboard || !PlayerInput.UsingGamepad;
            if (shouldDrawButton) {
                // 在交互按钮和对话之间一条浅黑线，如果没有交互按钮就不Draw了（绘制条件和交互按钮一致）
                byte breakPixel = 2; // 左右都有[breakPixel]个像素的空隙
                Main.spriteBatch.Draw(GreyPixel.Value, PanelPosition + new Vector2(breakPixel * 2, linePositioning * 30 - 30), null, Color.White * 0.9f, 0f, Vector2.Zero, new Vector2(TextureAssets.ChatBack.Width() - breakPixel * 4, 3f), SpriteEffects.None, 0f);
                // 按钮
                ButtonHandler.DrawButtons((int)(linePositioning * 30 - 30 + PanelPosition.Y), focusText, focusText2);
            }
        }

        /// <summary>绘制钱币、任务物品和快乐值之类的杂项显示</summary>
        internal static void DrawTextPanelExtra(Vector2 textPanelPosition, Vector2 textPanelSize, Rectangle panelRectangle, int money, int itemType) {
            Vector2 position = textPanelPosition + textPanelSize;

            if (money != 0) {
                // 钱币绘制，因原版代码问题要调一下位置
                ItemSlot.DrawMoney(Main.spriteBatch, "", position.X - 130, position.Y - 65, Utils.CoinsSplit(money), horizontal: true);
            }

            // 任务物品展示
            if (itemType != 0) {
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

            // 自己加的一个显示幸福值的小功能
            if (Main.LocalPlayer.sign == -1 && Main.npc.IndexInRange(Main.LocalPlayer.talkNPC)) {
                position = new Vector2((float)PanelPosition.X + panelRectangle.Width, (float)PanelPosition.Y - 18);
                var npc = Main.npc[Main.LocalPlayer.talkNPC];
                npc.GetNPCPreferenceSorted(out var NPCPreferences, out var biomePreferences);
                foreach (var preference in NPCPreferences) {
                    int head = NPC.TypeToDefaultHeadIndex(preference.NpcId);
                    if (NPC.AnyNPCs(preference.NpcId) && TownNPCProfiles.Instance.GetProfile(preference.NpcId, out ITownNPCProfile profile)) {
                        head = profile.GetHeadTextureIndex(Main.npc[NPC.FindFirstNPC(preference.NpcId)]);
                    }
                    if (head > 0 && head < NPCHeadLoader.NPCHeadCount) {
                        var texture = TextureAssets.NpcHead[head];
                        var origin = texture.Size() / 2f;
                        position.X -= texture.Width() + 4; // 调整到绘制位置，以实现一排排列的效果
                        var drawPos = new Vector2(position.X + origin.X, position.Y);
                        var outlineColor = DrawingHelper.AffectionLevelColor(preference.Level);

                        DrawingHelper.DrawIconWithOutline(texture.Value, drawPos, origin, outlineColor, npc);

                        Rectangle rect = new((int)(drawPos.X - origin.X - 2), (int)(drawPos.Y - origin.Y - 2), texture.Width() + 2, texture.Height() + 2);
                        if (rect.Contains(new Point(Main.mouseX, Main.mouseY))) {
                            DrawingHelper.DrawTextTopPanel($"{Language.GetTextValue($"Mods.{DialogueTweak.instance.Name}.{preference.Level}")}: {Lang.GetNPCNameValue(preference.NpcId)}", panelRectangle);
                        }
                    }
                }
                foreach (var biomes in biomePreferences) {
                    foreach (var biome in biomes.Preferences) {
                        Rectangle frame = DrawingHelper.ShopBiomeVanillaIconFrame(biome.Biome, BiomeIconTags);
                        var texture = BiomeIconTags.Value;
                        var name = ShopHelper.BiomeNameByKey(biome.Biome.NameKey);
                        // 对于模组群系，如果可以的话直接获取BestiaryIcon
                        if (biome.Biome is ModBiome modBiome) {
                            if (modBiome.BestiaryIcon != null && modBiome.Mod.HasAsset(modBiome.BestiaryIcon)) {
                                texture = modBiome.Mod.Assets.Request<Texture2D>(modBiome.BestiaryIcon).Value;
                                frame = new(0, 0, 30, 30); // tML限制了icon必须为30x30
                            }
                            name = modBiome.DisplayName.GetTranslation(Language.ActiveCulture);
                        }
                        var origin = frame.Size() / 2f;
                        position.X -= frame.Width + 4; // 调整到绘制位置，以实现一排排列的效果
                        var drawPos = new Vector2(position.X + origin.X, position.Y);
                        var outlineColor = DrawingHelper.AffectionLevelColor(biome.Affection);

                        DrawingHelper.DrawIconWithOutline(texture, drawPos, origin, outlineColor, npc, frame);

                        Rectangle rect = new((int)(drawPos.X - origin.X - 2), (int)(drawPos.Y - origin.Y - 2), frame.Width + 2, frame.Height + 2);
                        if (rect.Contains(new Point(Main.mouseX, Main.mouseY))) {
                            DrawingHelper.DrawTextTopPanel($"{Language.GetTextValue($"Mods.{DialogueTweak.instance.Name}.{biome.Affection}")}: {name}", panelRectangle);
                        }
                    }
                }
            }
        }

        internal static void DrawTextAndPanel(Vector2 textPanelPosition, Vector2 textPanelSize, Color textColor, float letterAppeared, int amountOfLines, string[] textLines) {
            ButtonHandler.DrawPanel(Main.spriteBatch, ChatTextPanel.Value,
                textPanelPosition, // position
                textPanelSize, // size
                ChatTextPanelColor, cornerSize: 12, barSize: 4);

            // 对话文本
            int letterCount = 0;
            for (int i = 0; i < amountOfLines; i++) {
                string text = textLines[i];
                if (text != null) {
                    var snippets = ChatManager.ParseMessage(text, textColor).ToArray();
                    ChatManager.ConvertNormalSnippets(snippets);
                    foreach (var snippet in snippets) {
                        letterCount += snippet.Text.Length; // 直接这一段加进去
                        if (letterCount > letterAppeared) {
                            // 计算出多了多少个
                            int outOfRange = letterCount - (int)letterAppeared;
                            snippet.Text = (outOfRange >= snippet.Text.Length) ? string.Empty : snippet.Text.Remove(snippet.Text.Length - outOfRange);
                        }
                    }
                    var font = FontAssets.MouseText.Value;
                    var basePos = textPanelPosition + new Vector2(5, 5 + i * 30);
                    // 输入法缓冲文本与光标闪动
                    if (i == amountOfLines - 1 && Main.editSign) {
                        var drawCursor = basePos + new Vector2(ChatManager.GetStringSize(font, snippets, Vector2.One).X, 0);
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
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, basePos, 0, Vector2.Zero, Vector2.One, out _);
                    // 也进行braek退出显示
                    if (letterCount > letterAppeared) {
                        break;
                    }
                }
            }
        }

        internal static void DrawPanel(float linePositioning, Color color, out Rectangle rectangle) {
            // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接用最大行数限制吧
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value,
                position: PanelPosition,
                sourceRectangle: new Rectangle(0, 0, TextureAssets.ChatBack.Width(), (int)(linePositioning >= 15 ? 450 : 2 + linePositioning * 30)),
                color: color);
            if (linePositioning >= 15) {
                Main.spriteBatch.Draw(
                    texture: TextureAssets.ChatBack.Value,
                    position: PanelPosition + new Vector2(0, 450f),
                    sourceRectangle: new Rectangle(0, 30, TextureAssets.ChatBack.Width(), (int)(2 + (linePositioning - 15) * 30)),
                    color: color);
            }
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value, position: PanelPosition + new Vector2(0, 2 + linePositioning * 30),
                sourceRectangle: new Rectangle(0, TextureAssets.ChatBack.Height() - 30, TextureAssets.ChatBack.Width(), 30),
                color: color);
            rectangle = new Rectangle((int)PanelPosition.X, (int)PanelPosition.Y, TextureAssets.ChatBack.Width(), (int)(2 + linePositioning * 30 + 30));
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

        /// <summary>决定按钮文本以及用于绘制的基于行数的定位</summary>
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

        internal static void PrepareCache(ref TextDisplayCache textDisplayCache, out string[] textLines, out int amountOfLines) {
            textDisplayCache.PrepareCache(Main.npcChatText); // 处理对话文本（换行、统计行数之类）
            textLines = textDisplayCache.TextLines;
            amountOfLines = textDisplayCache.AmountOfLines;
        }
    }
}
