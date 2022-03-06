namespace DialogueTweak.Interfaces
{
    internal class PortraitDrawer : ModSystem
    {
        public static event Action<SpriteBatch, Color, Rectangle> OnPortraitDraw;
        public static event Action<SpriteBatch, Color, Rectangle, NPC> OnPreNPCPortraitDraw;
        public static event Action<SpriteBatch, Color, Rectangle, NPC> OnPostNPCPortraitDraw;
        public static event Action<SpriteBatch, Color, Rectangle, int> OnPreSignPortraitDraw;
        public static event Action<SpriteBatch, Color, Rectangle, int> OnPostSignPortraitDraw;

        public override void Load() {
            OnPortraitDraw += DrawNPCPortrait;
            OnPortraitDraw += DrawSignPortrait;
        }

        public override void Unload() {
            OnPortraitDraw = null;
            OnPreNPCPortraitDraw = null;
            OnPostNPCPortraitDraw = null;
            OnPreSignPortraitDraw = null;
            OnPostSignPortraitDraw = null;
        }

        // NPC肖像
        private void DrawNPCPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
            if (Main.LocalPlayer.talkNPC < 0 || Main.LocalPlayer.sign != -1 || Main.npc[Main.LocalPlayer.talkNPC] is null || !Main.npc[Main.LocalPlayer.talkNPC].active) {
                return;
            }

            var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];
            var frame = talkNPC.frame;
            float scale = 1.8f;

            // Pre
            if (OnPreNPCPortraitDraw is not null)
                OnPreNPCPortraitDraw.Invoke(sb, textColor, panel, talkNPC);

            // 1.4得用这个，城镇宠物有多种品种，用这个才能确保获得对话NPC的贴图
            Texture2D value = TextureAssets.Npc[talkNPC.type].Value;
            if (TownNPCProfiles.Instance.GetProfile(talkNPC.type, out ITownNPCProfile profile))
                value = profile.GetTextureNPCShouldUse(talkNPC).Value;

            // ModCall
            foreach (var info in from a in HandleAssets.IconInfos where a.npcTypes.Contains(talkNPC.type) && a.available() && a.texture != "" && a.iconType == IconType.Portrait select a) {
                if (info.texture == "None") return;
                value = ModContent.Request<Texture2D>(info.texture).Value;
                frame = (info.frame ?? (() => new Rectangle(0, 0, value.Width, value.Height)))(); // 如果info.frame为null则使用new Rectangle(0, 0, value.Width, value.Height)
            }

            var position = panel.Location.ToVector2() + new Vector2(62f, 62f);
            var origin = new Vector2(frame.Width, frame.Height) / 2f;

            // NPC绘制部分，重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

            sb.Draw(value, position, frame, talkNPC.GetAlpha(Color.White), 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);
            if (talkNPC.color != default(Color))
                sb.Draw(value, position, frame, talkNPC.GetColor(talkNPC.color), 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);

            // 名字，用DeathText因为它大而且清晰
            Utils.DrawBorderStringFourWay(sb, FontAssets.DeathText.Value, talkNPC.GivenOrTypeName, 270f + (Main.screenWidth - 800) / 2, 106, textColor, Color.Black, Vector2.Zero, 0.6f);

            // 还原
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

            // Post
            if (OnPostNPCPortraitDraw is not null)
                OnPostNPCPortraitDraw.Invoke(sb, textColor, panel, talkNPC);
        }

        // 标牌
        private void DrawSignPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
            if (Main.LocalPlayer.sign == -1) {
                return;
            }

            // 画像
            var value = HandleAssets.SignIcon.Value;
            Rectangle? frame = null;
            bool useItemTexture = true;
            int i = Main.LocalPlayer.sign;
            if (Main.sign[i] is null || !WorldGen.InWorld(Main.sign[i].x, Main.sign[i].y) || !Main.tile[Main.sign[i].x, Main.sign[i].y].HasTile)
                return;

            // Pre
            if (OnPreSignPortraitDraw is not null)
                OnPreSignPortraitDraw.Invoke(sb, textColor, panel, i);

            // ModCall
            foreach (var info in from a in HandleAssets.IconInfos where a.npcTypes.Contains(-1) && a.available() && a.texture != "" && a.iconType == IconType.Portrait select a) {
                if (info.texture == "None") return;
                value = ModContent.Request<Texture2D>(info.texture).Value;
                frame = info.frame();
                useItemTexture = false;
            }

            // 名字与图像
            string text = Lang._mapLegendCache.FromTile(Main.Map[Main.sign[i].x, Main.sign[i].y], Main.sign[i].x, Main.sign[i].y);
            if (useItemTexture) {
                var tile = Main.tile[Main.sign[i].x, Main.sign[i].y];
                WorldGen.KillTile_GetItemDrops(Main.sign[i].x, Main.sign[i].y, tile, out int dropItem, out int _, out _, out int _); // 获取物品
                if (dropItem == 0) ChatMethods.GetSignItemType(Main.sign[i].x, Main.sign[i].y, tile.TileType, out dropItem);
                if (dropItem < TextureAssets.Item.Length && TextureAssets.Item[dropItem] is not null) {
                    Main.instance.LoadItem(dropItem);
                    value = TextureAssets.Item[dropItem].Value;
                    var item = new Item();
                    item.netDefaults(dropItem);
                    text = item.Name;
                }
            }

            var position = panel.Location.ToVector2() + new Vector2(62f, 65f);
            // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

            sb.Draw(value, position, frame, Color.White, 0f, value.Size() / 2f, 2f, SpriteEffects.None, 0f);
            // 名字
            Utils.DrawBorderStringFourWay(sb, FontAssets.DeathText.Value, text, 270f + (Main.screenWidth - 800) / 2, 106, textColor, Color.Black, Vector2.Zero, 0.6f);

            // 还原
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

            // Post
            if (OnPostSignPortraitDraw is not null)
                OnPostSignPortraitDraw.Invoke(sb, textColor, panel, i);
        }

        internal static void DrawPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
            if (OnPortraitDraw is not null)
                OnPortraitDraw.Invoke(sb, textColor, panel);
        }
    }
}
