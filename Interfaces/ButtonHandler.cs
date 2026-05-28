using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace DialogueTweak.Interfaces;

internal class ButtonHandler
{
    private static SpriteBatch SpriteBatch => Main.spriteBatch;
    private static int MouseX => Main.mouseX;
    private static int MouseY => Main.mouseY;
    private static int ScreenWidth => Main.screenWidth;

    public static Asset<Texture2D> ButtonPanel;
    public static Asset<Texture2D> ButtonPanel_Highlight;

    private static bool moveOnBackButton;
    private static bool moveOnHappinessButton;
    private static bool moveOnHousingButton;
    private static Dictionary<NPCInteraction, bool> _interactionHoverStates = new();

    public const int SideButtonSize = 44;

    public static void DrawButtons(int statY) {
        int talk = Main.LocalPlayer.talkNPC;

        bool isHomeless = talk >= 0 && NPC.CanShowHomelessText(talk);
        bool hasHappinessReport = Main.LocalPlayer.sign == -1 &&
                                  Main.LocalPlayer.currentShoppingSettings.HappinessReport != "" &&
                                  Main.npc[talk].townNPC;

        bool showAnySideButton = isHomeless || hasHappinessReport;
        // 返回按钮，小动物由于没有幸福值/住房按钮，所以返回按钮要长一点。由于返回按钮总会显示，就不考虑手柄了
        DrawBackButton(statY, !showAnySideButton);
        if (moveOnBackButton && Main.mouseLeft && Main.mouseLeftRelease) return;

        if (isHomeless) {
            DrawHousingButton(statY);
        }
        else if (hasHappinessReport) {
            DrawHappinessButton(statY);
        }
        else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = false;

        // 获取该NPC的所有可用原生交互按钮
        int type;
        if (Main.LocalPlayer.sign != -1) {
            type = 0; // native sign registration type
        }
        else {
            type = Main.npc[talk].type;
        }

        var entries = Main.NPCInteractionDB.GetInteractionEntries(type);
        if (entries == null) return;

        // 过滤：排除侧按钮对应的实例，只保留启用且满足条件的
        var drawEntries = entries.Where(e =>
            e.Enabled &&
            e.NPCInteraction.Condition() &&
            e.NPCInteraction != NPCInteractionDatabase.CloseButton &&
            e.NPCInteraction != NPCInteractionDatabase.HappinessButton &&
            e.NPCInteraction != NPCInteractionDatabase.HousingButton
        ).ToList();

        if (drawEntries.Count == 0) return;

        int spacing = 10;
        int firstRowMax = showAnySideButton ? 2 : 3; // 有侧按钮时第一行最多2个，没有侧按钮时最多3个
        int otherRowMax = 3; // 后续行最多3个

        // 第一行主按钮区域：从侧按钮右侧到面板右侧，宽度375
        const int firstRowStartX = 122; // offset from PanelPosition.X
        const int firstRowWidth = 365;
        // 后续行：占用整行（包括侧按钮位置），左右边界对齐第一行
        const int otherRowStartX = 16; // offset from PanelPosition.X (侧按钮位置)
        int otherRowWidth = firstRowWidth + (firstRowStartX - otherRowStartX); // 481

        int entryIndex = 0;
        int row = 0;
        int head = -1;
        if (talk >= 0) {
            var npc = Main.npc[talk];
            head = !TownNPCProfiles.Instance.GetProfile(npc, out var profile)
                ? NPC.TypeToDefaultHeadIndex(type)
                : profile.GetHeadTextureIndex(npc);
        }

        while (entryIndex < drawEntries.Count) {
            int maxThisRow = row == 0 ? firstRowMax : otherRowMax;
            int remaining = drawEntries.Count - entryIndex;
            int countInRow = Math.Min(remaining, maxThisRow);

            int rowStartX = row == 0 ? firstRowStartX : otherRowStartX;
            int rowWidth = row == 0 ? firstRowWidth : otherRowWidth;
            int buttonWidth = (rowWidth - (countInRow - 1) * spacing) / countInRow;

            float rowY = statY + 10 + row * 54; // 每行高度44 + 10间距
            var bottom = new Vector2(ScreenWidth / 2f, rowY + 54);

            for (int col = 0; col < countInRow; col++) {
                var interaction = drawEntries[entryIndex].NPCInteraction;
                var pos = new Vector2(ChatUI.PanelPosition.X + rowStartX + col * (buttonWidth + spacing), rowY);
                string text = interaction.GetText();

                ChatMethods.ResolveInteractionIcon(interaction, type, head,
                    out var texAsset, out var frame, out var customOffset);

                _interactionHoverStates.TryAdd(interaction, false);
                bool hoverState = _interactionHoverStates[interaction];

                DrawMainButton(frame, texAsset.Value, pos, bottom, buttonWidth, text.Trim(),
                    interaction, customOffset, ref hoverState);

                _interactionHoverStates[interaction] = hoverState;
                entryIndex++;
            }

            row++;
        }
    }

    #region 主按钮绘制

    /// <summary>
    /// 绘制主按钮
    /// </summary>
    /// <param name="frame">图标的帧(sourceRectangle)</param>
    /// <param name="tex">图标的贴图</param>
    /// <param name="drawPosition">整个按钮绘制的左上角</param>
    /// <param name="panelBottom">整个对话框面板的底部</param>
    /// <param name="width">按钮的宽</param>
    /// <param name="buttonText">按钮显示的文字</param>
    /// <param name="interaction">按钮对应的NPCInteraction</param>
    /// <param name="moveOnButton">判断是否移动到按钮上的bool字段</param>
    private static void DrawMainButton(Rectangle frame, Texture2D tex, Vector2 drawPosition, Vector2 panelBottom,
        int width, string buttonText, NPCInteraction interaction, Func<float> customTextOffset,
        ref bool moveOnButton) {
        bool useText = !string.IsNullOrWhiteSpace(buttonText); // 确实有文本
        bool useIcon = tex is not null;
        int height = 44;
        var size = new Vector2(width, height);
        var customOffset = customTextOffset?.Invoke();

        // 按钮背景
        DrawPanel(SpriteBatch, ButtonPanel.Value, drawPosition, size, Color.White);

        // 对应图像（即icon）
        var iconOffset = Vector2.Zero;
        if (useIcon) {
            // If offset is there, the fixed offset will be ignored and the icon is centered on the space between the left border and the text(offset)
            if (customOffset is not null) {
                iconOffset.X = customOffset.Value;
                // 高度居中，但左侧固定和框有个距离
                var origin = frame.Size() / 2f;
                var iconPosition = drawPosition + new Vector2(customOffset.Value, height) / 2f;

                SpriteBatch.Draw(tex, iconPosition, frame, Color.White * 0.9f, 0f, origin, 1f,
                    SpriteEffects.None, 0f);
            }
            else {
                // 图标偏移
                iconOffset = new Vector2(22, height) / 2f;
                if (!useText) {
                    // 没有文本时，图标居中
                    iconOffset.X = (width - frame.Width) / 2f;
                }

                // 高度居中，但左侧固定和框有个距离
                var origin = new Vector2(0f, frame.Height / 2f);

                SpriteBatch.Draw(tex, drawPosition + iconOffset, frame, Color.White * 0.9f, 0f, origin, 1f,
                    SpriteEffects.None, 0f);

                // 为文本绘制作准备
                iconOffset.X += frame.Width + 4f;
            }
        }

        var buttonRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, width, height);
        MainButtonLogic(buttonRectangle, drawPosition, size, HoverOnButtonCallback, ref moveOnButton);
        
        drawPosition.X += iconOffset.X;

        // 还有一个文字提示
        if (useText) {
            // 为什么要-8? 去除按钮板的边框大小
            width -= (int)iconOffset.X + 8;
            if (interaction.ShowExcalmation) {
                width -= 10;
            }
            height -= 8;
            buttonRectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, width, height);
            MainButtonText(buttonRectangle, buttonText, panelBottom, moveOnButton, customOffset is not null,
                out var textEnd);
            drawPosition = textEnd;
        }
        else {
            drawPosition.Y += height / 2f + 4;
        }

        if (interaction.ShowExcalmation) {
            Utils.DrawNotificationIcon(SpriteBatch, drawPosition + new Vector2(4, 0), 0f);
        }
        
        return;
        void HoverOnButtonCallback() => HandleInteractionClick(interaction);
    }

    private static void MainButtonLogic(Rectangle buttonRectangle, Vector2 pos, Vector2 size,
        Action hoverOnButtonCallback, ref bool moveOnButton) {
        if (buttonRectangle.Contains(Main.MouseScreen.ToPoint())) {
            if (!moveOnButton) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                moveOnButton = true;
            }

            // 高光边框
            DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, size, Color.White);
            Main.LocalPlayer.mouseInterface = true;

            hoverOnButtonCallback.Invoke();
        }
        else if (moveOnButton) {
            moveOnButton = false;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private static void MainButtonText(Rectangle boundingBox, string buttonText, Vector2 panelBottom,
        bool moveOnButton, bool useCustomOffset, out Vector2 endPosition) {
        var textColor = new Color(Main.mouseTextColor, (int)(Main.mouseTextColor / 1.1), Main.mouseTextColor / 2,
            Main.mouseTextColor);
        var shadowColor = !moveOnButton ? Color.Black : Color.Brown;
        var font = FontAssets.MouseText.Value;
        float scaleX = DecideTextScale(buttonText, font, boundingBox.Width);
        var scale = new Vector2(scaleX, 1f);
        var stringSize = ChatManager.GetStringSize(font, buttonText, scale);
        
        var pos = boundingBox.Center();
        pos.X -= stringSize.X / 2f;
        pos.Y -= font.LineSpacing / 4f;
        if (!useCustomOffset && stringSize.X < boundingBox.Width * 0.7f && stringSize.X < 100f) {
            pos.X -= 10f - stringSize.X * 0.1f;
        }

        DrawButtonText(buttonText, moveOnButton ? 2 : 1.5f, shadowColor, textColor, scale, pos);
        endPosition = pos + new Vector2(stringSize.X + 4, stringSize.Y / 2f - 2);

        if (scaleX <= 0.7f && moveOnButton) {
            // 缩放程度太高的放在上面时会在面板下方显示文本
            panelBottom.X -= ChatManager.GetStringSize(font, buttonText, Vector2.One).X / 2f;
            DrawButtonText(buttonText, 1.2f, Color.Black, textColor, Vector2.One, panelBottom);
        }
    }

    private static void HandleInteractionClick(NPCInteraction interaction) {
        if (!Main.mouseLeft || !Main.mouseLeftRelease) return;

        if (NPCLoader.PreChatButtonClicked(interaction)) {
            interaction.Interact();
            NPCLoader.OnChatButtonClicked(interaction);
        }
    }

    #endregion

    #region 侧按钮 (住房、幸福度和返回按钮)

    private static void DrawBackButton(float statY, bool longer) {
        Rectangle buttonRectangle =
            new Rectangle((int)ChatUI.PanelPosition.X + 16, (int)statY + 10, longer ? 98 : 44, 44);
        var value = ModAsset.Button_Back.Value;
        Rectangle frame = value.Frame();
        // ModCall
        int type = Main.LocalPlayer.sign != -1 ? 0 : Main.npc[Main.LocalPlayer.talkNPC].type;
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != ""
                       && a.InteractionTypeName == "CloseChat"
                 select a) {
            value = ModContent.Request<Texture2D>(info.Texture).Value;
            frame = info.Frame?.Invoke() ?? value.Frame();
        }

        DrawPanel(SpriteBatch, ButtonPanel.Value, buttonRectangle.Location.ToVector2(), buttonRectangle.Size(),
            Color.White);
        SpriteBatch.Draw(value, buttonRectangle.Center(), frame,
            Color.White * 0.9f, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

        if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
            if (!moveOnBackButton) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                moveOnBackButton = true;
            }

            DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, buttonRectangle.Location.ToVector2(),
                buttonRectangle.Size(), Color.White);
            Main.LocalPlayer.mouseInterface = true;

            if (Main.mouseLeft && Main.mouseLeftRelease) {
                NPCInteractionDatabase.CloseButton.Interact();
            }
        }
        else if (moveOnBackButton) {
            moveOnBackButton = false;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        if (NPCInteractionDatabase.CloseButton.ShowExcalmation) {
            Utils.DrawNotificationIcon(SpriteBatch, buttonRectangle.TopRight() + new Vector2(-10f, 20f), 0f);
        }

        // 手柄支持，这个是最左边
        UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat0,
            buttonRectangle.Location.ToVector2() + buttonRectangle.Size() / 2f);
        UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsLeft = true;
    }

    private static void DrawHappinessButton(float statY) {
        Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 68, statY + 10);
        var value = ModAsset.Button_Happiness.Value;
        Rectangle frame = value.Frame();

        // ModCall
        int type = Main.LocalPlayer.sign != -1 ? 0 : Main.npc[Main.LocalPlayer.talkNPC].type;
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != ""
                       && a.InteractionTypeName == "ReportHappiness"
                 select a) {
            value = ModContent.Request<Texture2D>(info.Texture).Value;
            frame = info.Frame?.Invoke() ?? value.Frame();
        }

        DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(SideButtonSize), Color.White);

        var center = pos + new Vector2(SideButtonSize) / 2f;
        SpriteBatch.Draw(value, center, frame, Color.White * 0.9f, 0f, value.Size() / 2f, 1f, SpriteEffects.None, 0f);

        Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, SideButtonSize, SideButtonSize);
        if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
            if (!moveOnHappinessButton) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                moveOnHappinessButton = true;
            }

            DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(44, 44), Color.White);
            Main.LocalPlayer.mouseInterface = true;

            if (Main.mouseLeft && Main.mouseLeftRelease) {
                NPCInteractionDatabase.HappinessButton.Interact();
                TryDisplayNPCPreferences();
            }
        }
        else if (moveOnHappinessButton) {
            moveOnHappinessButton = false;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        if (NPCInteractionDatabase.HappinessButton.ShowExcalmation) {
            Utils.DrawNotificationIcon(SpriteBatch, buttonRectangle.TopRight() + new Vector2(-10f, 20f), 0f);
        }

        // 手柄支持，这个是左中
        UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos + buttonRectangle.Size() / 2f);
        UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
    }

    private static void DrawHousingButton(float statY) {
        Vector2 pos = new Vector2(ChatUI.PanelPosition.X + 68, statY + 10);
        var value = ModContent.Request<Texture2D>("Terraria/Images/UI/DisplaySlots_5").Value;
        Rectangle frame = value.Frame();

        // ModCall
        int type = Main.LocalPlayer.sign != -1 ? 0 : Main.npc[Main.LocalPlayer.talkNPC].type;
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != ""
                       && a.InteractionTypeName == "RequestHome"
                 select a) {
            value = ModContent.Request<Texture2D>(info.Texture).Value;
            frame = info.Frame?.Invoke() ?? value.Frame();
        }

        DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(44, 44), Color.White);

        var center = pos + new Vector2(SideButtonSize) / 2f;
        SpriteBatch.Draw(value, center, frame, Color.White * 0.9f, 0f, value.Size() / 2f, 1f, SpriteEffects.None, 0f);

        Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, SideButtonSize, SideButtonSize);
        if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
            if (!moveOnHousingButton) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                moveOnHousingButton = true;
            }

            DrawPanel(SpriteBatch, ButtonPanel_Highlight.Value, pos, new Vector2(44, 44), Color.White);
            Main.LocalPlayer.mouseInterface = true;

            if (Main.mouseLeft && Main.mouseLeftRelease) {
                SoundEngine.PlaySound(SoundID.MenuTick);
                NPCInteractionDatabase.HousingButton.Interact();
            }
        }
        else if (moveOnHousingButton) {
            moveOnHousingButton = false;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        if (NPCInteractionDatabase.HousingButton.ShowExcalmation) {
            Utils.DrawNotificationIcon(SpriteBatch, buttonRectangle.TopRight() + new Vector2(-10f, 20f), 0f);
        }

        // 手柄支持，这个是左中
        UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos + buttonRectangle.Size() / 2f);
        UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
    }

    private static void TryDisplayNPCPreferences() {
        if (!Configuration.Instance.DisplayPreference) return;

        // 点击按钮后在左下角显示具体偏好情况
        var npc = Main.npc[Main.LocalPlayer.talkNPC];
        npc.GetNPCPreferenceSorted(out var npcPreferences, out var biomePreferences);
        Main.NewText($"[c/{Main.DiscoColor.Hex3()}:{NPC.GetFullnameByID(npc.type)}]");
        foreach (var preference in npcPreferences) {
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

    #endregion

    private static float DecideTextScale(string text, DynamicSpriteFont font, float maxWidth) {
        Vector2 stringSize = ChatManager.GetStringSize(font, text, Vector2.One); // 先计算出一般情况下(即scale为1)的大小
        if (stringSize.X <= maxWidth) return 1f; // 能容纳的直接给过
        return 1f * (maxWidth / stringSize.X); // 不能容纳的进行缩放，最小不能超过0.3
    }

    private static void DrawButtonText(string text, float spread, Color shadowColor, Color chatColor, Vector2 scale,
        Vector2 pos) {
        var font = FontAssets.MouseText.Value;
        var array = ChatManager.ParseMessage(text, chatColor);
        ChatManager.ConvertNormalSnippets(array);
        ChatManager.DrawColorCodedStringShadow(SpriteBatch, font, array, pos, shadowColor, 0f, Vector2.Zero, scale, -1,
            spread);
        ChatManager.DrawColorCodedString(SpriteBatch, font, array, pos, chatColor, 0f, Vector2.Zero, scale, out int _,
            -1);
    }

    public static void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 size,
        Color color, Color? cornerColor = null, int cornerSize = 6, int barSize = 32) {
        Color corner = cornerColor ?? color;
        Point point = new Point((int)position.X, (int)position.Y);
        Point point2 = new Point(point.X + (int)size.X - cornerSize, point.Y + (int)size.Y - cornerSize);
        int width = point2.X - point.X - cornerSize;
        int height = point2.Y - point.Y - cornerSize;
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, cornerSize, cornerSize),
            new Rectangle(0, 0, cornerSize, cornerSize), corner); // left-top corner
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, cornerSize, cornerSize),
            new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), corner); // right-top corner
        spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, cornerSize, cornerSize),
            new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), corner); // left-bottom corner
        spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize),
            new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize),
            corner); // right-bottom corner
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize),
            new Rectangle(cornerSize, 0, barSize, cornerSize), color); // top bar
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize),
            new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color); // bottom bar
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height),
            new Rectangle(0, cornerSize, cornerSize, barSize), color); // left bar
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height),
            new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color); // right bar
        spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height),
            new Rectangle(cornerSize, cornerSize, barSize, barSize), color); // middle bar
    }
}