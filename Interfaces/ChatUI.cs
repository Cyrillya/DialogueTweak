using ReLogic.Localization.IME;
using ReLogic.OS;
using Terraria.GameContent.UI.Chat;

namespace DialogueTweak.Interfaces
{
    public class ChatUI : UIState
    {
        private static TextDisplayCache _textDisplayCache = new();

        internal static Asset<Texture2D> BiomeIconTags;
        internal static Asset<Texture2D> PortraitPanel;
        internal static Asset<Texture2D> ChatTextPanel;
        internal static Asset<Texture2D> GreyPixel;
        public static readonly Color ChatTextPanelColor = new(35, 43, 89);

        internal static bool CursorAtTextPanel;
        internal static string PrevText;
        internal static float LetterAppeared;
        internal static float TotalLetters;
        public const int MAX_LINES = 16;

        public static Vector2 PanelPosition => new(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100f);
        public static float LineSpacing => FontAssets.MouseText.Value.LineSpacing;

        public override void Draw(SpriteBatch spriteBatch) {
            if (Main.LocalPlayer.talkNPC < 0 && Main.LocalPlayer.sign == -1) {
                Main.npcChatText = "";
                return;
            }

            int panelValue = 230;
            Color panelColor = new(panelValue, panelValue, panelValue, panelValue);
            int textValue = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new(textValue, textValue, textValue, textValue);

            PrepareCache(ref _textDisplayCache, out TextSnippet[] snippets, out int amountOfLines);
            if (Main.editSign) {
                PrepareBlinker(ref Main.instance.textBlinkerCount, ref Main.instance.textBlinkerState);
                // 输入法面板，不过1.4tml好像不调用游戏内输入法面板了
                Main.instance.DrawWindowsIMEPanel(new Vector2(Main.screenWidth / 2, 90f), 0.5f);
            }
            PrepareVirtualKeyboard(amountOfLines);
            PrepareLinesFocuses(ref amountOfLines, out string focusText, out string focusText2, out int money, out float linePositioning);
            DrawPanel(linePositioning, panelColor, out Rectangle rectangle);

            var textPanelPosition = PanelPosition + new Vector2(120, 45f);
            var textPanelSize = new Vector2(370f, amountOfLines * LineSpacing);

            DrawTextAndPanel(textPanelPosition, textPanelSize, LetterAppeared, snippets); // 文字框
            // 人像背景框，以及名字和文本的分割线
            Main.spriteBatch.Draw(PortraitPanel.Value, PanelPosition + new Vector2(0, 15f), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 钱币、物品、偏好之类
            DrawTextPanelExtra(textPanelPosition, textPanelSize, rectangle, money, Main.npcChatCornerItem);
            // 肖像
            PortraitDrawer.DrawPortrait(Main.spriteBatch, textColor, rectangle);
            DrawButtons(focusText, focusText2, linePositioning);

            // 判断鼠标是否处于对话栏界面
            if (new Rectangle((int)textPanelPosition.X, (int)textPanelPosition.Y, (int)textPanelSize.X, (int)textPanelSize.Y).Contains(new Point(Main.mouseX, Main.mouseY))) {
                CursorAtTextPanel = true;
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
                Main.spriteBatch.Draw(GreyPixel.Value, PanelPosition + new Vector2(breakPixel * 2, linePositioning * LineSpacing - LineSpacing), null, Color.White * 0.9f, 0f, Vector2.Zero, new Vector2(TextureAssets.ChatBack.Width() - breakPixel * 4, 3f), SpriteEffects.None, 0f);

                // 按钮
                ButtonHandler.DrawButtons((int)(linePositioning * LineSpacing - LineSpacing + PanelPosition.Y), focusText, focusText2);
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
                    if (NPC.AnyNPCs(preference.NpcId) && TownNPCProfiles.Instance.GetProfile(Main.npc[NPC.FindFirstNPC(preference.NpcId)], out ITownNPCProfile profile)) {
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
                            if (modBiome.BestiaryIcon != null && ModContent.HasAsset(modBiome.BestiaryIcon)) {
                                texture = ModContent.Request<Texture2D>(modBiome.BestiaryIcon, AssetRequestMode.ImmediateLoad).Value;
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

        internal static void DrawTextAndPanel(Vector2 textPanelPosition, Vector2 textPanelSize, float letterAppeared, TextSnippet[] originalSnippets) {
            ButtonHandler.DrawPanel(Main.spriteBatch, ChatTextPanel.Value,
                textPanelPosition, // position
                textPanelSize, // size
                ChatTextPanelColor, cornerSize: 12, barSize: 4);

            // 对话文本
            int linesCount = 0;
            int letterCount = 0;
            List<TextSnippet> snippets = new(); // new一个新的出来，不然就把原来的snippets改了
            foreach (var snippet in originalSnippets) {
                if (snippet is PlainTagHandler.PlainSnippet) {
                    letterCount += snippet.Text.Length; // 直接这一段加进去
                    string visualString = snippet.Text;
                    if (letterCount > letterAppeared) {
                        // 计算出多了多少个
                        int outOfRange = letterCount - (int)letterAppeared;
                        visualString = (outOfRange >= snippet.Text.Length) ? string.Empty : snippet.Text.Remove(snippet.Text.Length - outOfRange);
                    }
                    linesCount += visualString.Split('\n').Length - 1;
                    if (linesCount > MAX_LINES) {
                        visualString = string.Empty;
                    }
                    if (visualString != string.Empty) {
                        snippets.Add(new TextSnippet(visualString, snippet.Color));
                    }
                }
                else {
                    letterCount += 1; // 直接这一段加进去
                    if (letterCount <= letterAppeared) {
                        snippets.Add(snippet);
                    }
                }
            }
            var font = FontAssets.MouseText.Value;
            var basePos = textPanelPosition + new Vector2(5);
            // 输入法缓冲文本与光标闪动
            if (Main.editSign && linesCount <= MAX_LINES) {
                string compositionString = Platform.Get<IImeService>().CompositionString;
                if (compositionString != null && compositionString.Length > 0) {
                    snippets.Add(new TextSnippet(compositionString, new Color(255, 240, 20)));
                }
                if (Main.instance.textBlinkerState == 1) {
                    snippets.Add(new TextSnippet("|"));
                }
            }
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets.ToArray(), basePos, 0, Vector2.Zero, Vector2.One, out _);
        }

        internal static void DrawPanel(float linePositioning, Color color, out Rectangle rectangle) {
            // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接让他写出格吧
            linePositioning = Math.Clamp(linePositioning, 0f, 22f);
            int height = (int)(2 + linePositioning * LineSpacing);
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value,
                position: PanelPosition,
                sourceRectangle: new Rectangle(0, 0, TextureAssets.ChatBack.Width(), (int)(linePositioning >= 15 ? LineSpacing * 15 : height)),
                color: color);
            if (linePositioning >= 15) {
                Main.spriteBatch.Draw(
                    texture: TextureAssets.ChatBack.Value,
                    position: PanelPosition + new Vector2(0, LineSpacing * 15),
                    sourceRectangle: new Rectangle(0, (int)LineSpacing, TextureAssets.ChatBack.Width(), (int)(2 + (linePositioning - 15) * LineSpacing)),
                    color: color);
            }
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value, position: PanelPosition + new Vector2(0, height),
                sourceRectangle: new Rectangle(0, TextureAssets.ChatBack.Height() - 32, TextureAssets.ChatBack.Width(), 32),
                color: color);
            rectangle = new Rectangle((int)PanelPosition.X, (int)PanelPosition.Y, TextureAssets.ChatBack.Width(), height + 32);
        }

        // Text scrolling 文字滚动机制
        public override void Update(GameTime gameTime) {
            if (Main.npcChatText != PrevText) {
                LetterAppeared = 0;
            }
            if (Main.LocalPlayer.sign > -1) {
                LetterAppeared = 1145141919; // 标牌没有缓慢出现机制
                return;
            }
            PrevText = Main.npcChatText;
            if (!Main.npc.IndexInRange(Main.LocalPlayer.talkNPC) || Main.npc[Main.LocalPlayer.talkNPC] is null || !Main.npc[Main.LocalPlayer.talkNPC].active) {
                return;
            }
            if (LetterAppeared < TotalLetters + 1) {
                float speakingRateMultipiler = GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive ? 2.5f : 4f;
                if (CursorAtTextPanel) {
                    speakingRateMultipiler *= 1.2f;
                    if (Main.mouseLeft) {
                        speakingRateMultipiler *= 3f; // 快速吟唱
                    }
                }
                LetterAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * speakingRateMultipiler;
            }
            CursorAtTextPanel = false;
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
            float offsetDown = 120 + amountOfLines * LineSpacing + LineSpacing;
            offsetDown -= 180;
            UIVirtualKeyboard.ShouldHideText = !PlayerInput.UsingGamepad;
            if (!PlayerInput.UsingGamepad) // 不使用手柄就隐藏了
                offsetDown = 9999;
            UIVirtualKeyboard.OffsetDown = (int)offsetDown;
        }

        /// <summary>决定按钮文本以及用于绘制的基于行数的定位</summary>
        internal static void PrepareLinesFocuses(ref int amountOfLines, out string focusText, out string focusText2, out int money, out float linePositioning) {
            amountOfLines++;

            // 判断一下：如果有钱币显示（税收官，护士之类的）则多加一行
            focusText = "";
            focusText2 = "";
            money = 0;
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref money);
            if (money != 0)
                amountOfLines++;

            if (Main.npcChatCornerItem > 0)
                amountOfLines++;

            linePositioning = (amountOfLines <= 2 ? 2.2f : amountOfLines) + 3f; // float更方便细调，其实就是为了手柄编辑标牌的对称感
            if (Main.editSign && !UIVirtualKeyboard.ShouldHideText) { // 手柄下编辑标牌时底部没有按钮，会缩回去
                linePositioning -= 1.85f;
            }
        }

        internal static void PrepareCache(ref TextDisplayCache textDisplayCache, out TextSnippet[] snippets, out int amountOfLines) {
            textDisplayCache.PrepareCache(Main.npcChatText); // 处理对话文本（换行、统计行数之类）
            snippets = textDisplayCache.Snippets;
            amountOfLines = textDisplayCache.AmountOfLines;
        }
    }
}
