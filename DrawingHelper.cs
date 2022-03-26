namespace DialogueTweak
{
    internal static class DrawingHelper
    {
        public static Rectangle ShopBiomeVanillaIconFrame(IShoppingBiome biome, Asset<Texture2D> texture) {
            if (biome is OceanBiome) {
                return texture.Frame(16, 5, 12, 1);
            }
            if (biome is ForestBiome) {
                return texture.Frame(16, 5, 0, 0);
            }
            if (biome is SnowBiome) {
                return texture.Frame(16, 5, 5, 0);
            }
            if (biome is DesertBiome) {
                return texture.Frame(16, 5, 4, 0);
            }
            if (biome is JungleBiome) {
                return texture.Frame(16, 5, 6, 1);
            }
            if (biome is UndergroundBiome) {
                return texture.Frame(16, 5, 2, 0);
            }
            if (biome is HallowBiome) {
                return texture.Frame(16, 5, 1, 1);
            }
            if (biome is MushroomBiome) {
                return texture.Frame(16, 5, 8, 1);
            }
            if (biome is DungeonBiome) {
                return texture.Frame(16, 5, 0, 2);
            }
            if (biome is CorruptionBiome) {
                return texture.Frame(16, 5, 7, 0);
            }
            if (biome is CrimsonBiome) {
                return texture.Frame(16, 5, 12, 0);
            }
            return texture.Frame(16, 5, 0, 4); // 简单的一个“?”
        }

        /// <summary>根据偏好不同描边颜色不同</summary>
        public static Color AffectionLevelColor(AffectionLevel level) {
            switch (level) {
                case AffectionLevel.Love:
                    return Color.LawnGreen;
                case AffectionLevel.Like:
                    return Color.CornflowerBlue;
                case AffectionLevel.Dislike:
                    return Color.BurlyWood;
                case AffectionLevel.Hate:
                    return Color.MediumVioletRed;
            }
            return Color.White;
        }

        /// <summary>在面板上方绘制文字</summary>
        public static void DrawTextTopPanel(string text, Rectangle panelRectangle) {
            Vector2 top = new Vector2(panelRectangle.X + panelRectangle.Width, panelRectangle.Y + 12f);
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            var stringSize = ChatManager.GetStringSize(font, text, Vector2.One);
            top.X -= stringSize.X;
            Color textColor = new Color(Main.mouseTextColor, (int)((double)Main.mouseTextColor / 1.1), Main.mouseTextColor / 2, Main.mouseTextColor);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, top, Color.Black, 0f, Vector2.Zero, Vector2.One, -1, 1.5f);
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, top, textColor, 0f, Vector2.Zero, Vector2.One);
        }

        internal static void DrawIconWithOutline(Texture2D tex, Vector2 center, Vector2 origin, Color outlineColor, Entity shaderEntity, Rectangle? sourceRectangle = null) {
            // 描边
            Main.spriteBatch.End(); // End后Begin来使用shader绘制描边
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
            GameShaders.Armor.Apply(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, shaderEntity, null);
            for (int k = -1; k <= 1; k++) {
                for (int l = -1; l <= 1; l++) {
                    if (Math.Abs(k) + Math.Abs(l) == 1) {
                        var offset = new Vector2((float)k * 2f, (float)l * 2f);
                        Main.spriteBatch.Draw(tex, center + offset, sourceRectangle, outlineColor, 0f, origin, 1f, SpriteEffects.None, 0f);
                    }
                }
            }
            Main.spriteBatch.End(); // End之后Begin恢复原状
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

            // 原贴图绘制
            Main.spriteBatch.Draw(tex, center, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
