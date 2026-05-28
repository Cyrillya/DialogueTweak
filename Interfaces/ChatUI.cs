using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Localization.IME;
using ReLogic.OS;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using Terraria.UI.Chat;

namespace DialogueTweak.Interfaces;

public class ChatUI : UIState
{
    private static TextDisplayCache _textDisplayCache = new();

    internal static Asset<Texture2D> BiomeIconTags;
    internal static Asset<Texture2D> ChatTextPanel;
    public static readonly Color ChatTextPanelColor = new(35, 43, 89);

    internal static bool CursorAtTextPanel;
    internal static string PrevText;
    internal static float LetterAppeared;
    internal static float TotalLetters;
    public const int MAX_LINES = 16;

    public static Vector2 PanelPosition => new(Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2, 100f);
    public static float LineSpacing => FontAssets.MouseText.Value.LineSpacing;

    private NPCChatPanel _chatPanel => Main.instance._newChatPanel;

    public override void Draw(SpriteBatch spriteBatch) {
        UpdateTextScrolling();

        if (Main.LocalPlayer.talkNPC < 0 && Main.LocalPlayer.sign == -1) {
            Main.npcChatText = "";
            return;
        }

        if (!_chatPanel.CanHoldConversation()) {
            _chatPanel.Close();
            return;
        }

        // _chatPanel.PrepareText();
        PrepareText(ref _textDisplayCache, out List<TextSnippet> snippets, out int amountOfLines, out float lastLineLength);
        _chatPanel.PrepareInteractions();
        // _chatPanel.PrepareVirtualKeyboard();
        PrepareVirtualKeyboard(amountOfLines);

        int panelAlpha = 230;
        Color panelColor = new(panelAlpha, panelAlpha, panelAlpha, panelAlpha);
        int textAlpha = (Main.mouseTextColor * 2 + 255) / 3;
        Color textColor = new(textAlpha, textAlpha, textAlpha, textAlpha);

        if (Main.editSign) {
            PrepareBlinker(ref Main.instance.textBlinkerCount, ref Main.instance.textBlinkerState);

            if (Platform.Get<IImeService>().CompositionString is { Length: > 0 }) {
                Main.instance.SetIMEPanelAnchor(new Vector2(Main.screenWidth / 2f, 90f), 0.5f);
            }
        }

        PrepareLinesFocuses(lastLineLength, ref amountOfLines, out int money, out float linePositioning, out int buttonRows);
        DrawPanel(linePositioning, panelColor, buttonRows, out var rectangle);

        var textPanelPosition = PanelPosition + new Vector2(116, 42f);
        var textPanelSize = new Vector2(378f, amountOfLines * LineSpacing + 8f);

        DrawTextAndPanel(textPanelPosition, textPanelSize, LetterAppeared, snippets); // 文字框
        // 人像框背景，以及名字和文本的分割线
        Main.spriteBatch.Draw(ModAsset.PortraitPanel_Seperate.Value, PanelPosition + new Vector2(0, 15f), null,
            Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        // 钱币、物品、偏好之类
        DrawTextPanelExtra(textPanelPosition, textPanelSize, rectangle, money, Main.npcChatCornerItem,
            out int preferenceTextWidth);
        // 肖像
        PortraitDrawer.DrawPortrait(Main.spriteBatch, textColor, rectangle);
        // NPC/标牌名字
        DrawName(Main.spriteBatch, textColor, rectangle, preferenceTextWidth);
        DrawButtons(linePositioning);

        // 判断鼠标是否处于对话栏界面
        if (new Rectangle((int)textPanelPosition.X, (int)textPanelPosition.Y, (int)textPanelSize.X,
                (int)textPanelSize.Y).Contains(new Point(Main.mouseX, Main.mouseY))) {
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

    internal static void DrawButtons(float linePositioning) {
        bool shouldDrawButton = Main.InGameUI.CurrentState is not UIVirtualKeyboard || !PlayerInput.UsingGamepad;
        if (!shouldDrawButton) return;

        float fontFixOffset = 28f - LineSpacing;

        // 在交互按钮和对话之间一条浅黑线，如果没有交互按钮就不Draw了（绘制条件和交互按钮一致）
        Main.spriteBatch.Draw(ModAsset.ButtonSeperator.Value,
            PanelPosition + new Vector2(0, linePositioning * LineSpacing - LineSpacing + fontFixOffset),
            Color.White * 0.9f);

        // 按钮
        ButtonHandler.DrawButtons((int)(linePositioning * LineSpacing - LineSpacing + fontFixOffset + PanelPosition.Y));
    }

    internal static void DrawName(SpriteBatch sb, Color textColor, Rectangle panel, int preferenceTextWidth) {
        string text = null;
        if (Main.LocalPlayer.talkNPC >= 0 && Main.LocalPlayer.sign == -1 &&
            Main.npc[Main.LocalPlayer.talkNPC] is not null && Main.npc[Main.LocalPlayer.talkNPC].active) {
            var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
            text = talkNPC.GivenOrTypeName;
        }

        if (Main.LocalPlayer.sign != -1) {
            int i = Main.LocalPlayer.sign;
            if (Main.sign[i] is not null && WorldGen.InWorld(Main.sign[i].x, Main.sign[i].y) &&
                Main.tile[Main.sign[i].x, Main.sign[i].y].HasTile) {
                text = Lang._mapLegendCache.FromTile(Main.Map[Main.sign[i].x, Main.sign[i].y], Main.sign[i].x,
                    Main.sign[i].y);
            }
        }

        if (string.IsNullOrWhiteSpace(text))
            return;

        float x = 270f + (Main.screenWidth - 800) / 2;
        float y = 108;
        float width = panel.Right - x;
        float textWidth = FontAssets.DeathText.Value.MeasureString(text).X * 0.54f;

        // don't show name text if it overlaps with preference text
        if (preferenceTextWidth + textWidth > width)
            return;

        Utils.DrawBorderStringFourWay(sb, FontAssets.DeathText.Value, text, x, y, textColor, Color.Black, Vector2.Zero,
            0.54f);
    }

    /// <summary>绘制钱币、任务物品和快乐值之类的杂项显示</summary>
    internal static void DrawTextPanelExtra(Vector2 textPanelPosition, Vector2 textPanelSize, Rectangle panelRectangle,
        int money, int itemType, out int preferenceTextWidth) {
        preferenceTextWidth = 0;
        Vector2 textPanelRightBottom = textPanelPosition + textPanelSize;

        if (money != 0) {
            // 钱币绘制，因原版代码问题要调一下位置
            ItemSlot.DrawMoney(Main.spriteBatch, "", textPanelRightBottom.X - 130, textPanelRightBottom.Y - 65,
                Utils.CoinsSplit(money), horizontal: true);
        }

        // 任务物品展示
        if (itemType != 0) {
            Main.DrawNPCChatBottomRightItem(textPanelRightBottom + new Vector2(4f));
        }

        // 1.4.5的显示幸福值功能
        if (Configuration.Instance.DisplayHappiness && Main.LocalPlayer.sign == -1) {
            // 1.4.5原版数据
            int shopHappinessTextOffsetX = 26;
            int shopHappinessTextOffsetY = 98;
            int shopHappinessIconOffsetX = 12;
            int shopHappinessIconOffsetY = 108;
            // 本模组修正值
            int offsetX = 4;
            int offsetY = -122;
            // 最终值
            int textOffsetX = shopHappinessTextOffsetX + offsetX;
            int textOffsetY = shopHappinessTextOffsetY + offsetY;
            int iconOffsetX = shopHappinessIconOffsetX + offsetX;
            int iconOffsetY = shopHappinessIconOffsetY + offsetY;


            Texture2D texture = Main.Assets.Request<Texture2D>("Images/UI/NPCHappiness").Value;

            double priceAdjustment = Main.LocalPlayer.currentShoppingSettings.PriceAdjustment;

            int frameX = ((!(priceAdjustment <= 0.82f))
                ? ((priceAdjustment <= 1f) ? 1 : ((!(priceAdjustment <= 1.1f)) ? 3 : 2))
                : 0);
            Rectangle frame = texture.Frame(4, 1, frameX);

            Vector2 iconPos = new Vector2(PanelPosition.X + iconOffsetX, PanelPosition.Y + iconOffsetY);
            Main.spriteBatch.Draw(texture, iconPos, frame, Color.White, 0f, frame.Size() / 2f, 1f, SpriteEffects.None,
                0f);

            string happinessText = priceAdjustment.ToString("P0");
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, happinessText,
                PanelPosition.X + textOffsetX, PanelPosition.Y + textOffsetY,
                Color.White * ((float)(int)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero);
        }


        // 自己加的一个显示幸福值的小功能
        bool preferenceHovered = false;
        if (Configuration.Instance.DisplayPreference && Main.LocalPlayer.sign == -1 &&
            Main.npc.IndexInRange(Main.LocalPlayer.talkNPC)) {
            textPanelRightBottom =
                new Vector2((float)PanelPosition.X + panelRectangle.Width, (float)PanelPosition.Y - 18);
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            npc.GetNPCPreferenceSorted(out var NPCPreferences, out var biomePreferences);
            foreach (var preference in NPCPreferences) {
                int head = NPC.TypeToDefaultHeadIndex(preference.NpcId);
                if (NPC.AnyNPCs(preference.NpcId) &&
                    TownNPCProfiles.Instance.GetProfile(Main.npc[NPC.FindFirstNPC(preference.NpcId)],
                        out ITownNPCProfile profile)) {
                    head = profile.GetHeadTextureIndex(Main.npc[NPC.FindFirstNPC(preference.NpcId)]);
                }

                if (head > 0 && head < NPCHeadLoader.NPCHeadCount) {
                    var texture = TextureAssets.NpcHead[head];
                    var origin = texture.Size() / 2f;
                    textPanelRightBottom.X -= texture.Width() + 4; // 调整到绘制位置，以实现一排排列的效果
                    var drawPos = new Vector2(textPanelRightBottom.X + origin.X, textPanelRightBottom.Y);
                    var outlineColor = DrawingHelper.AffectionLevelColor(preference.Level);

                    DrawingHelper.DrawIconWithOutline(texture.Value, drawPos, origin, outlineColor, npc);

                    Rectangle rect = new((int)(drawPos.X - origin.X - 2), (int)(drawPos.Y - origin.Y - 2),
                        texture.Width() + 4, texture.Height() + 2);
                    if (rect.Contains(new Point(Main.mouseX, Main.mouseY))) {
                        preferenceHovered = true;
                        DrawingHelper.DrawTextTopPanel(
                            $"{Language.GetTextValue($"Mods.{DialogueTweak.Instance.Name}.{preference.Level}")}: {Lang.GetNPCNameValue(preference.NpcId)}",
                            panelRectangle, out preferenceTextWidth);
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
                            texture = ModContent
                                .Request<Texture2D>(modBiome.BestiaryIcon, AssetRequestMode.ImmediateLoad).Value;
                            frame = new(0, 0, 30, 30); // tML限制了icon必须为30x30
                        }

                        name = modBiome.DisplayName.Value;
                    }

                    var origin = frame.Size() / 2f;
                    textPanelRightBottom.X -= frame.Width + 4; // 调整到绘制位置，以实现一排排列的效果
                    var drawPos = new Vector2(textPanelRightBottom.X + origin.X, textPanelRightBottom.Y);
                    var outlineColor = DrawingHelper.AffectionLevelColor(biome.Affection);

                    DrawingHelper.DrawIconWithOutline(texture, drawPos, origin, outlineColor, npc, frame);

                    Rectangle rect = new((int)(drawPos.X - origin.X - 2), (int)(drawPos.Y - origin.Y - 2),
                        frame.Width + 4, frame.Height + 2);
                    if (rect.Contains(new Point(Main.mouseX, Main.mouseY))) {
                        preferenceHovered = true;
                        DrawingHelper.DrawTextTopPanel(
                            $"{Language.GetTextValue($"Mods.{DialogueTweak.Instance.Name}.{biome.Affection}")}: {name}",
                            panelRectangle, out preferenceTextWidth);
                    }
                }
            }
        }

        // 切换回原版UI的按钮，当悬停在幸福度图标上面时不显示防止文字重叠
        if (Configuration.Instance.ShowSwapButton && !preferenceHovered) {
            DrawingHelper.DrawGUISwapButton(panelRectangle.TopRight());
        }
    }

    internal static void DrawTextAndPanel(Vector2 textPanelPosition, Vector2 textPanelSize, float letterAppeared,
        List<TextSnippet> originalSnippets) {
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
                    visualString = (outOfRange >= snippet.Text.Length)
                        ? string.Empty
                        : snippet.Text.Remove(snippet.Text.Length - outOfRange);
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
        var basePos = textPanelPosition + new Vector2(7, 8);

        // 输入法缓冲文本与光标闪动
        if (Main.editSign && linesCount <= MAX_LINES) {
            string compositionString = Platform.Get<IImeService>().CompositionString;
            if (compositionString is { Length: > 0 }) {
                snippets.Add(new TextSnippet(compositionString, new Color(255, 240, 20)));
            }

            if (Main.instance.textBlinkerState == 1) {
                snippets.Add(new TextSnippet("|"));
            }
        }

        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets.ToArray(), basePos, 0, Vector2.Zero,
            Vector2.One, out _);
    }

    internal static void DrawPanel(float linePositioning, Color color, int buttonRows, out Rectangle rectangle) {
        // 没事别乱改参数了呜呜呜，这里是背景板。中间加一段防止文字太长了的延续特判，当然再长我就不管了，直接让他写出格吧
        linePositioning = Math.Clamp(linePositioning, 0f, 22f);
        int height = (int)(2 + linePositioning * LineSpacing - (LineSpacing - 28f) * 3.4f);
        // 额外按钮行扩展面板高度
        if (buttonRows > 1)
            height += (buttonRows - 1) * 54;
        Main.spriteBatch.Draw(
            texture: TextureAssets.ChatBack.Value,
            position: PanelPosition,
            sourceRectangle: new Rectangle(0, 0, TextureAssets.ChatBack.Width(),
                (int)(linePositioning >= 15 ? LineSpacing * 15 : height)),
            color: color);
        if (linePositioning >= 15) {
            Main.spriteBatch.Draw(
                texture: TextureAssets.ChatBack.Value,
                position: PanelPosition + new Vector2(0, LineSpacing * 15),
                sourceRectangle: new Rectangle(0, (int)LineSpacing, TextureAssets.ChatBack.Width(),
                    (int)(2 + (linePositioning - 15) * LineSpacing)),
                color: color);
        }

        Main.spriteBatch.Draw(
            texture: TextureAssets.ChatBack.Value, position: PanelPosition + new Vector2(0, height),
            sourceRectangle: new Rectangle(0, TextureAssets.ChatBack.Height() - 32, TextureAssets.ChatBack.Width(), 32),
            color: color);
        rectangle = new Rectangle((int)PanelPosition.X, (int)PanelPosition.Y, TextureAssets.ChatBack.Width(),
            height + 32);
    }

    // Text scrolling 文字滚动机制
    public void UpdateTextScrolling() {
        if (Main.npcChatText != PrevText) {
            LetterAppeared = 0;
            PortraitDrawer.DoNPCPortraitHop();
            PrevText = Main.npcChatText;
        }

        if (Main.LocalPlayer.sign > -1 ||
            Configuration.Instance.TextScrollingMode is Configuration.TextScrollingSpeed.Disabled) {
            LetterAppeared = 1145141919; // 标牌没有缓慢出现机制
            return;
        }

        if (!Main.npc.IndexInRange(Main.LocalPlayer.talkNPC) || Main.npc[Main.LocalPlayer.talkNPC] is null ||
            !Main.npc[Main.LocalPlayer.talkNPC].active) {
            return;
        }

        if (LetterAppeared < TotalLetters + 1) {
            float speakingRateMultipiler =
                GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive ? 1.25f : 2f;
            switch (Configuration.Instance.TextScrollingMode) {
                case Configuration.TextScrollingSpeed.Slow:
                    speakingRateMultipiler *= 0.5f;
                    break;
                case Configuration.TextScrollingSpeed.Fast:
                    speakingRateMultipiler *= 1.5f;
                    break;
            }

            if (CursorAtTextPanel) {
                if (Main.mouseRight) {
                    speakingRateMultipiler *= 3f; // 快速吟唱
                }
            }

            LetterAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) *
                              speakingRateMultipiler * DialogueTweakSystem.CurrentRefreshRateFactor;
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
    internal static void PrepareLinesFocuses(float lastLineLength, ref int amountOfLines,
        out int money, out float linePositioning, out int buttonRows) {
        amountOfLines++;

        money = 0;
        int drawableCount = 0;
        int interactionType;
        if (Main.LocalPlayer.sign != -1) {
            interactionType = 0; // native sign registration type
        }
        else {
            interactionType = Main.npc[Main.LocalPlayer.talkNPC].type;
        }

        var allEntries = Main.NPCInteractionDB.GetInteractionEntries(interactionType);
        if (allEntries != null) {
            foreach (var entry in allEntries) {
                if (!entry.Enabled || !entry.NPCInteraction.Condition()) continue;
                Color c = Color.White;
                int coins = 0;
                entry.NPCInteraction.TryAddCoins(ref c, out coins);
                if (coins != 0 && money == 0) {
                    money = coins;
                }

                // 统计可绘制的主按钮数量（排除侧按钮）
                if (entry.NPCInteraction != NPCInteractionDatabase.CloseButton &&
                    entry.NPCInteraction != NPCInteractionDatabase.HappinessButton &&
                    entry.NPCInteraction != NPCInteractionDatabase.HousingButton) {
                    drawableCount++;
                }
            }
        }

        if (money != 0 && lastLineLength > 240)
            amountOfLines++;

        if (Main.npcChatCornerItem > 0 && lastLineLength > 300)
            amountOfLines++;

        // 计算按钮行数
        bool showAnySideButton = Main.LocalPlayer.talkNPC >= 0 &&
            (NPC.CanShowHomelessText(Main.LocalPlayer.talkNPC) ||
             (Main.LocalPlayer.sign == -1 && Main.LocalPlayer.currentShoppingSettings.HappinessReport != "" &&
              Main.npc[Main.LocalPlayer.talkNPC].townNPC));
        int firstRowMax = showAnySideButton ? 2 : 3;
        buttonRows = drawableCount == 0 ? 0 :
            drawableCount <= firstRowMax ? 1 :
            1 + (int)Math.Ceiling((double)(drawableCount - firstRowMax) / 3);

        linePositioning = amountOfLines + 3f;
        if (amountOfLines <= 2) {
            linePositioning = 5.2f;
        }

        // 由于UI的位置是行距和行数决定的，所以这里要做一些特判，防止玩家自用字体导致的UI错位
        if (linePositioning * LineSpacing < 5.2f * 28f) {
            linePositioning = 5.2f * 28f / LineSpacing;
        }

        if (Main.editSign && !UIVirtualKeyboard.ShouldHideText) { // 手柄下编辑标牌时底部没有按钮，会缩回去
            linePositioning -= 1.85f;
        }
    }

    private void PrepareText(ref TextDisplayCache textDisplayCache, [CanBeNull] out List<TextSnippet> snippets, out int amountOfLines,
        out float lastLineLength) {
        string chatTextToShow = Main.npcChatText;
        _chatPanel.OverrideChatTextWithShenanigans(ref chatTextToShow);
        _textDisplayCache.PrepareCache(chatTextToShow);
        textDisplayCache.PrepareCache(chatTextToShow); // 处理对话文本（换行、统计行数之类）
        snippets = textDisplayCache.Snippets;
        amountOfLines = textDisplayCache.AmountOfLines;
        lastLineLength = textDisplayCache.LastLineLength;
    }
}