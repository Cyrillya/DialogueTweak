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

    public static Asset<Texture2D> Shop;
    private static Rectangle _shopFrame;
    private static Func<float> _shopCustomOffset;
    public static Asset<Texture2D> Extra;
    private static Rectangle _extraFrame;
    private static Func<float> _extraCustomOffset;

    private static bool moveOnBackButton;
    private static bool moveOnHappinessButton;
    private static bool moveOnShopButton;
    private static bool moveOnExtraButton;

    public static void DrawButtons(int statY, string focusText, string focusText2) {
        int talk = Main.LocalPlayer.talkNPC;
        NPCLoader.SetChatButtons(ref focusText, ref focusText2);

        bool showHappinessReport = Main.LocalPlayer.sign == -1 &&
                                   Main.LocalPlayer.currentShoppingSettings.HappinessReport != "" &&
                                   Main.npc[Main.LocalPlayer.talkNPC].townNPC;
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
        foreach (int i in from a in HandleAssets.ButtonInfos
                 where a.NPCTypes.Contains(type) && a.Available()
                 select HandleAssets.ButtonInfos.IndexOf(a))
            buttons.Add(i);
        int buttonCounts = buttons.Count + useShopButton.ToInt() + useExtraButton.ToInt();
        if (buttonCounts == 0) return;
        int spacing = 10; // 按扭之间的间隔
        int buttonWidth = 375 / buttonCounts - spacing; // 每个按钮的宽度，+2是加上，-10是去除了按钮之间的间隔

        // 决定图标
        ChatMethods.HandleButtonIcon(type, out Shop, out _shopFrame, out _shopCustomOffset, out Extra, out _extraFrame,
            out _extraCustomOffset);

        int offsetX = 0;
        var bottom = new Vector2(ScreenWidth / 2f, statY + 64);

        Vector2 GetDrawPosition() => new(ChatUI.PanelPosition.X + 122 + offsetX, statY + 10);

        if (useExtraButton) {
            var pos = GetDrawPosition();
            DrawMainButton(_extraFrame, Extra.Value, pos, bottom, buttonWidth, focusText2.Trim(), ExtraButtonCallback,
                _shopCustomOffset, ref moveOnExtraButton);
            offsetX += buttonWidth + spacing;

            // 手柄支持，这个是右中
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat2, pos);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false;
            }
        }
        else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false; // 考虑手柄

        if (useShopButton) {
            var pos = GetDrawPosition();
            DrawMainButton(_shopFrame, Shop.Value, pos, bottom, buttonWidth, focusText.Trim(), ShopButtonCallback,
                _shopCustomOffset, ref moveOnShopButton);
            offsetX += buttonWidth + spacing;

            // 手柄支持，这个是最右边
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat3, pos);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = false;
            }
        }
        else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 = false; // 考虑手柄

        foreach (var button in from i in buttons select HandleAssets.ButtonInfos[i]) {
            if (!button.Available.Invoke()) continue;

            var pos = GetDrawPosition();
            var text = button.ButtonText?.Invoke() ?? "";

            bool useIcon = button.IconTexture != "";
            var iconTexture = useIcon
                ? ModContent.Request<Texture2D>(button.IconTexture, AssetRequestMode.ImmediateLoad).Value
                : null;
            var frame = new Rectangle();
            if (useIcon) {
                frame = button.Frame?.Invoke() ?? new Rectangle(0, 0, iconTexture.Width, iconTexture.Height);
            }

            DrawMainButton(frame, iconTexture, pos, bottom, buttonWidth, text.Trim(), button.HoverAction,
                button.CustomOffset, ref button.Focused);
            offsetX += buttonWidth + spacing;
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
    /// <param name="hoverOnButtonCallback">鼠标悬停在按钮上时执行的事件</param>
    /// <param name="moveOnButton">判断是否移动到按钮上的bool字段</param>
    private static void DrawMainButton(Rectangle frame, Texture2D tex, Vector2 drawPosition, Vector2 panelBottom,
        int width, string buttonText, Action hoverOnButtonCallback, Func<float> customTextOffset,
        ref bool moveOnButton) {
        bool useText = !string.IsNullOrWhiteSpace(buttonText); // 确实有文本
        bool useIcon = tex is not null;
        int height = 44;
        var size = new Vector2(width, height);

        // 按钮
        DrawPanel(SpriteBatch, ButtonPanel.Value, drawPosition, size, Color.White);

        // 对应图像（即icon）
        var iconOffset = Vector2.Zero;
        if (useIcon) {
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

        var buttonRectangle = new Rectangle((int) drawPosition.X, (int) drawPosition.Y, width, height);
        MainButtonLogic(buttonRectangle, drawPosition, size, hoverOnButtonCallback, ref moveOnButton);

        // 还有一个文字提示
        if (useText) {
            iconOffset.X = customTextOffset?.Invoke() ?? iconOffset.X;
            drawPosition.X += iconOffset.X;
            // 为什么要-8? 去除按钮板的边框大小
            width -= (int) iconOffset.X + 8;
            height -= 8;
            buttonRectangle = new Rectangle((int) drawPosition.X, (int) drawPosition.Y, width, height);
            MainButtonText(buttonRectangle, buttonText, panelBottom, moveOnButton);
        }
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
        bool moveOnButton) {
        // Utils.DrawBorderString(SpriteBatch, "⏹", boundingBox.Location.ToVector2(), Color.White);
        // Utils.DrawBorderString(SpriteBatch, "⏹", boundingBox.BottomRight(), Color.White);

        var textColor = new Color(Main.mouseTextColor, (int) (Main.mouseTextColor / 1.1), Main.mouseTextColor / 2,
            Main.mouseTextColor);
        var shadowColor = !moveOnButton ? Color.Black : Color.Brown;
        var font = FontAssets.MouseText.Value;
        float scaleX = DecideTextScale(buttonText, font, boundingBox.Width);
        var scale = new Vector2(scaleX, 1f);
        var stringSize = ChatManager.GetStringSize(font, buttonText, scale);

        var pos = boundingBox.Center();
        pos.X -= stringSize.X / 2f;
        pos.Y -= font.LineSpacing / 4f;
        if (stringSize.X < boundingBox.Width * 0.7f && stringSize.X < 100f) {
            pos.X -= (10f - stringSize.X * 0.1f);
        }
        // Main.NewText(stringSize);

        DrawButtonText(buttonText, moveOnButton ? 2 : 1.5f, shadowColor, textColor, scale, pos);
        if (scaleX <= 0.7f && moveOnButton) {
            // 缩放程度太高的放在上面时会在面板下方显示文本
            panelBottom.X -= ChatManager.GetStringSize(font, buttonText, Vector2.One).X / 2f;
            DrawButtonText(buttonText, 1.2f, Color.Black, textColor, Vector2.One, panelBottom);
        }
    }

    private static void ExtraButtonCallback() {
        if (!Main.mouseLeft || !Main.mouseLeftRelease) return;

        ChatMethods.HandleExtraButtonClicled(Main.npc[Main.LocalPlayer.talkNPC]);
    }

    private static void ShopButtonCallback() {
        if (!Main.mouseLeft || !Main.mouseLeftRelease) return;

        if (Main.LocalPlayer.sign == -1)
            ChatMethods.HandleShop(Main.npc[Main.LocalPlayer.talkNPC]);
        else {
            if (Main.editSign)
                Main.SubmitSignText();
            else
                IngameFancyUI.OpenVirtualKeyboard(1);
        }
    }

    #endregion

    #region 侧按钮 (幸福度和返回按钮)

    private static void DrawBackButton(float statY, bool longer) {
        Rectangle buttonRectangle =
            new Rectangle((int) ChatUI.PanelPosition.X + 16, (int) statY + 10, longer ? 98 : 44, 44);
        var value = ModAsset.Button_Back.Value;
        Rectangle frame = value.Frame();
        // ModCall
        int type = Main.LocalPlayer.sign != -1 ? -1 : Main.npc[Main.LocalPlayer.talkNPC].type; // 为了标牌特判
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != "" && a.IconType == IconType.Back
                 select a) {
            value = ModContent.Request<Texture2D>(info.Texture).Value;
            frame = info.Frame?.Invoke() ?? value.Frame();
        }

        DrawPanel(SpriteBatch, ButtonPanel.Value, buttonRectangle.Location.ToVector2(), buttonRectangle.Size(),
            Color.White);
        SpriteBatch.Draw(value, buttonRectangle.Location.ToVector2() + buttonRectangle.Size() / 2f, frame,
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
                Main.CloseNPCChatOrSign();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }
        else if (moveOnBackButton) {
            moveOnBackButton = false;
            SoundEngine.PlaySound(SoundID.MenuTick);
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
        int type = Main.LocalPlayer.sign != -1 ? -1 : Main.npc[Main.LocalPlayer.talkNPC].type; // 为了标牌特判
        foreach (var info in from a in HandleAssets.IconInfos
                 where a.NPCTypes.Contains(type) && a.Available() && a.Texture != "" && a.IconType == IconType.Happiness
                 select a) {
            value = ModContent.Request<Texture2D>(info.Texture).Value;
            frame = info.Frame?.Invoke() ?? value.Frame();
        }

        DrawPanel(SpriteBatch, ButtonPanel.Value, pos, new Vector2(44, 44), Color.White);
        SpriteBatch.Draw(value, pos, frame, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        Rectangle buttonRectangle = new Rectangle((int) pos.X, (int) pos.Y, ModAsset.Button_Happiness.Width(),
            ModAsset.Button_Happiness.Height());
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
                TryDisplayNPCPreferences();
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
        var array = ChatManager.ParseMessage(text, chatColor).ToArray();
        ChatManager.ConvertNormalSnippets(array);
        ChatManager.DrawColorCodedStringShadow(SpriteBatch, font, array, pos, shadowColor, 0f, Vector2.Zero, scale, -1,
            spread);
        ChatManager.DrawColorCodedString(SpriteBatch, font, array, pos, chatColor, 0f, Vector2.Zero, scale, out int _,
            -1);
    }

    public static void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 size,
        Color color, Color? cornerColor = null, int cornerSize = 6, int barSize = 32) {
        Color corner = cornerColor ?? color;
        Point point = new Point((int) position.X, (int) position.Y);
        Point point2 = new Point(point.X + (int) size.X - cornerSize, point.Y + (int) size.Y - cornerSize);
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