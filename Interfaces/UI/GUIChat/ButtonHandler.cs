using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace DialogueTweak.Interfaces.UI.GUIChat
{
    internal class ButtonHandler
    {
        private static SpriteBatch SpriteBatch => Main.spriteBatch;
        private static int MouseX => Main.mouseX;
        private static int MouseY => Main.mouseY;
        private static int ScreenWidth => Main.screenWidth;

        public static Texture2D Button_Back;
        public static Texture2D Button_BackLong;
        public static Texture2D Button_Happiness;
        public static Texture2D Button_Highlight;

        public static Texture2D ButtonLong;
        public static Texture2D ButtonLong_Highlight;
        public static Texture2D ButtonLonger;
        public static Texture2D ButtonLonger_Highlight;

        public static Texture2D Shop;
        public static Texture2D Extra;

        private static bool moveOnBackButton;
        private static bool moveOnShopButton;
        private static bool moveOnExtraButton;

        public static void DrawButtons(float statY) {
            string focusText = "";
            string focusText2 = "";
            int num = (Main.mouseTextColor * 2 + 255) / 3;
            Color textColor = new Color(num, num, num, num);
            ChatMethods.HandleFocusText(ref focusText, ref focusText2, ref textColor, out int money);
            NPCLoader.SetChatButtons(ref focusText, ref focusText2);

            // 返回按钮。由于返回按钮总会显示，就不考虑手柄了
            DrawBackButton(statY);
            ChatMethods.HandleShopTexture(Main.LocalPlayer.sign != -1 ? -1 : Main.LocalPlayer.talkNPC, ref Shop, ref Extra);
            if (!string.IsNullOrWhiteSpace(focusText)) {
                DrawLongShopButton(statY, focusText, textColor, string.IsNullOrWhiteSpace(focusText2));
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false; // 考虑手柄
            if (!string.IsNullOrWhiteSpace(focusText2)) {
                DrawLongExtraButton(statY, focusText2, textColor);
            }
            else UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = false; // 考虑手柄
        }

        private static void DrawBackButton(float statY) {
            Vector2 pos = new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2 + 16, statY + 10);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, Button_BackLong.Width, Button_BackLong.Height / 2);
                SpriteBatch.Draw(Button_BackLong, pos, new Rectangle(0, 0, Button_BackLong.Width, Button_BackLong.Height / 2), Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnBackButton) {
                    Main.PlaySound(SoundID.MenuTick);
                    moveOnBackButton = true;
                }
                    SpriteBatch.Draw(Button_BackLong, pos, new Rectangle(0, 44, Button_BackLong.Width, Button_BackLong.Height / 2), Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    Main.CloseNPCChatOrSign();
                    Main.PlaySound(SoundID.MenuClose);
                }
            }
            else if (moveOnBackButton) {
                moveOnBackButton = false;
                Main.PlaySound(SoundID.MenuTick);
            }
            // 手柄支持，这个是最左边
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat0, pos + buttonRectangle.Size() / 2f);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsLeft = true;
        }

        private static void DrawLongShopButton(float statY, string shopText, Color chatColor, bool useLonger) {
            Texture2D asset = ButtonLong;
            Texture2D highlightAsset = ButtonLong_Highlight;
            Vector2 pos = new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2 + Button_Back.Width * 2 + asset.Width + 48, statY + 10);
            if (useLonger) {
                asset = ButtonLonger;
                highlightAsset = ButtonLonger_Highlight;
                pos = new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2 + Button_Back.Width * 2 + 34, statY + 10);
            }
            // 按钮
            SpriteBatch.Draw(asset, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 对应图像
            SpriteBatch.Draw(Shop, pos + new Vector2(asset.Height, asset.Height) / 2f, null, Color.White * 0.9f, 0f, new Vector2(Shop.Width, Shop.Height) / 2f, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, asset.Width, asset.Height);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                if (!moveOnShopButton) {
                    Main.PlaySound(SoundID.MenuTick);
                    moveOnShopButton = true;
                }
                SpriteBatch.Draw(highlightAsset, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    if (Main.LocalPlayer.sign == -1)
                        ChatMethods.HandleShop(Main.npc[Main.LocalPlayer.talkNPC].type);
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
                Main.PlaySound(SoundID.MenuTick);
            }
            // 还有一个文字提示
            DynamicSpriteFont value = Main.fontMouseText;
            Vector2 stringSize = ChatManager.GetStringSize(value, shopText, Vector2.One);
            Color shadowColor = (!moveOnShopButton) ? Color.Black : Color.Brown;
            Vector2 buttonOrigin = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
            Vector2 offset = new Vector2(0f, 4f);
            ChatManager.DrawColorCodedStringShadow(baseColor: shadowColor, spriteBatch: SpriteBatch, font: value, text: shopText, position: pos + buttonOrigin + offset, rotation: 0f, origin: stringSize * 0.5f, baseScale: Vector2.One);
            ChatManager.DrawColorCodedString(SpriteBatch, value, shopText, pos + buttonOrigin + offset, chatColor, 0f, stringSize * 0.5f, Vector2.One);

            // 手柄支持，这个是最右边
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat2, pos + buttonOrigin + offset);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos + buttonOrigin + offset);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false;
            }
        }
        private static void DrawLongExtraButton(float statY, string shopText, Color chatColor) {
            Texture2D asset = ButtonLong;
            Texture2D highlightAsset = ButtonLong_Highlight;
            Vector2 pos = new Vector2(ScreenWidth / 2 - Main.chatBackTexture.Width / 2 + Button_Back.Width * 2 + 34, statY + 10);
            // 按钮
            SpriteBatch.Draw(asset, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            // 对应图像
            SpriteBatch.Draw(Extra, pos + new Vector2(asset.Height, asset.Height) / 2f, null, Color.White * 0.9f, 0f, new Vector2(Extra.Width, Extra.Height) / 2f, 1f, SpriteEffects.None, 0f);
            Rectangle buttonRectangle = new Rectangle((int)pos.X, (int)pos.Y, asset.Width, asset.Height);
            if (buttonRectangle.Contains(new Point(MouseX, MouseY))) {
                NPC talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
                if (!moveOnExtraButton) {
                    Main.PlaySound(SoundID.MenuTick);
                    moveOnExtraButton = true;
                }
                SpriteBatch.Draw(highlightAsset, pos, null, Color.White * 0.9f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    ChatMethods.HandleExtraButtonClicled(talkNPC.type);
                }
            }
            else if (moveOnExtraButton) {
                moveOnExtraButton = false;
                Main.PlaySound(SoundID.MenuTick);
            }
            // 还有一个文字提示
            DynamicSpriteFont value = Main.fontMouseText;
            Vector2 stringSize = ChatManager.GetStringSize(value, shopText, Vector2.One);
            Color shadowColor = (!moveOnExtraButton) ? Color.Black : Color.Brown;
            Vector2 buttonOrigin = new Vector2(buttonRectangle.Width, buttonRectangle.Height) / 2f;
            Vector2 offset = new Vector2(0f, 4f);
            ChatManager.DrawColorCodedStringShadow(baseColor: shadowColor, spriteBatch: SpriteBatch, font: value, text: shopText, position: pos + buttonOrigin + offset, rotation: 0f, origin: stringSize * 0.5f, baseScale: Vector2.One);
            ChatManager.DrawColorCodedString(SpriteBatch, value, shopText, pos + buttonOrigin + offset, chatColor, 0f, stringSize * 0.5f, Vector2.One);
            // 手柄支持，这个是右中
            UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat2, pos + buttonOrigin + offset);
            UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = true;
            // 原版的奇妙操作，无论怎样NPC都会同时存在NPCChat0和NPCChat1的选项，这里用特判让第二个按钮定位到此按钮
            if (!UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle) {
                UILinkPointNavigator.SetPosition(GamepadPointID.NPCChat1, pos + buttonOrigin + offset);
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false;
            }
        }
    }
}
