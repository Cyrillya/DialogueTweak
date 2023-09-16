using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace DialogueTweak;

internal static class DrawingHelper
{
    public static Rectangle ShopBiomeVanillaIconFrame(IShoppingBiome biome, Asset<Texture2D> texture) {
        return biome switch {
            OceanBiome => texture.Frame(16, 5, 12, 1),
            ForestBiome => texture.Frame(16, 5, 0, 0),
            SnowBiome => texture.Frame(16, 5, 5, 0),
            DesertBiome => texture.Frame(16, 5, 4, 0),
            JungleBiome => texture.Frame(16, 5, 6, 1),
            UndergroundBiome => texture.Frame(16, 5, 2, 0),
            HallowBiome => texture.Frame(16, 5, 1, 1),
            MushroomBiome => texture.Frame(16, 5, 8, 1),
            DungeonBiome => texture.Frame(16, 5, 0, 2),
            CorruptionBiome => texture.Frame(16, 5, 7, 0),
            CrimsonBiome => texture.Frame(16, 5, 12, 0),
            _ => texture.Frame(16, 5, 0, 4)
        };
    }

    /// <summary>根据偏好不同描边颜色不同</summary>
    public static Color AffectionLevelColor(AffectionLevel level) {
        return level switch {
            AffectionLevel.Love => Color.LawnGreen,
            AffectionLevel.Like => Color.CornflowerBlue,
            AffectionLevel.Dislike => Color.BurlyWood,
            AffectionLevel.Hate => Color.MediumVioletRed,
            _ => Color.White
        };
    }

    /// <summary>在面板右上方绘制文字</summary>
    public static void DrawTextTopPanel(string text, Rectangle panelRectangle) {
        Vector2 top = new Vector2(panelRectangle.X + panelRectangle.Width, panelRectangle.Y + 12f);
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        var stringSize = ChatManager.GetStringSize(font, text, Vector2.One);
        top.X -= stringSize.X;
        Color textColor = new Color(Main.mouseTextColor, (int) ((double) Main.mouseTextColor / 1.1),
            Main.mouseTextColor / 2, Main.mouseTextColor);
        ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, top, Color.Black, 0f, Vector2.Zero,
            Vector2.One, -1, 1.5f);
        ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, top, textColor, 0f, Vector2.Zero, Vector2.One);
    }

    internal static void DrawIconWithOutline(Texture2D tex, Vector2 center, Vector2 origin, Color outlineColor,
        Entity shaderEntity, Rectangle? sourceRectangle = null) {
        // 描边
        Main.spriteBatch.End(); // End后Begin来使用shader绘制描边
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
        GameShaders.Armor.Apply(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, shaderEntity, null);
        for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
                if (Math.Abs(k) + Math.Abs(l) == 1) {
                    var offset = new Vector2((float) k * 2f, (float) l * 2f);
                    Main.spriteBatch.Draw(tex, center + offset, sourceRectangle, outlineColor, 0f, origin, 1f,
                        SpriteEffects.None, 0f);
                }
            }
        }

        Main.spriteBatch.End(); // End之后Begin恢复原状
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        // 原贴图绘制
        Main.spriteBatch.Draw(tex, center, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    internal static void DrawGUISwapButton(Vector2 topRightPosition) {
        const int horizontalFrames = 2;
        var tex = ModAsset.StyleSwapButton.Value;
        var position = topRightPosition;
        position.X -= tex.Width / horizontalFrames;
        position += new Vector2(-2, 2);
        var hitbox = new Rectangle((int) position.X, (int) position.Y, tex.Width / horizontalFrames, tex.Height);
        bool isHovered = hitbox.Contains(Main.MouseScreen.ToPoint());
        var frameRectangle = tex.Frame(horizontalFrames, frameX: isHovered.ToInt());
        Main.spriteBatch.Draw(tex, position, frameRectangle, Color.White);

        if (!isHovered) return;

        var config = Configuration.Instance;
        string key = $"Mods.DialogueTweak.UISwitch.To{(config.VanillaUI ? "Modded" : "Vanilla")}";
        UICommon.TooltipMouseText(Language.GetTextValue(key));

        if (Main.mouseLeft) {
            SoundEngine.PlaySound(SoundID.Chat);
            config.VanillaUI = !config.VanillaUI;
            ConfigManager.Save(config);
        }
    }
}