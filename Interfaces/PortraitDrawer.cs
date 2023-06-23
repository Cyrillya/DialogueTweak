using System.Reflection;
using Terraria.GameContent.Bestiary;
using Terraria.ObjectData;

namespace DialogueTweak.Interfaces;

internal class PortraitDrawer : ModSystem
{
    public UnlockableNPCEntryIcon icon;
        
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

        // Pre
        OnPreNPCPortraitDraw?.Invoke(sb, textColor, panel, talkNPC);

        var position = panel.Location.ToVector2();

        // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

        var previewBox = new Rectangle((int)position.X + 17, (int)position.Y + 18, 92, 94);

        if (talkNPC.active) {
            if (Configuration.Instance.BestiryPortrait) {
                icon ??= new UnlockableNPCEntryIcon(talkNPC.type) {
                    _npcCache = {scale = 2f}
                };

                if (icon._npcNetId != talkNPC.type) {
                    icon = new UnlockableNPCEntryIcon(talkNPC.type);
                }

                DrawNPCInBestiary(sb, previewBox);
            }
            else {
                DrawNPCRenderedInWorld(sb, talkNPC, previewBox);
            }
        }

        // 还原
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        // 名字，用DeathText因为它大而且清晰
        Utils.DrawBorderStringFourWay(sb, FontAssets.DeathText.Value, talkNPC.GivenOrTypeName, 270f + (Main.screenWidth - 800) / 2, 108, textColor, Color.Black, Vector2.Zero, 0.54f);

        // Post
        OnPostNPCPortraitDraw?.Invoke(sb, textColor, panel, talkNPC);
    }

    private void DrawNPCRenderedInWorld(SpriteBatch sb, NPC talkNPC, Rectangle previewBox) {
        int npcSize = Math.Max(talkNPC.height, talkNPC.width) + 20;
        int offset = npcSize / 2;
        float extraZoom = Main.GameZoomTarget - 1f;

        // 根据重力方向调整位置
        var effects = SpriteEffects.None;
        var source = new Rectangle((int)talkNPC.Center.X - offset, (int)talkNPC.Center.Y - offset - 6, npcSize, npcSize);
        source.Offset((-Main.screenPosition).ToPoint());
        if (Main.LocalPlayer.gravDir is -1) {
            source.Y = Main.screenHeight - source.Y - npcSize;
            effects = SpriteEffects.FlipVertically;
        }
                
        // 根据缩放调整位置
        int inflateValue = (int)(offset * extraZoom);
        source.Inflate(inflateValue, inflateValue);
        var screenOffset = (talkNPC.Center - Main.Camera.Center) * extraZoom;
        source.Offset((int)screenOffset.X, (int) (screenOffset.Y * Main.LocalPlayer.gravDir));
                
        sb.Draw(Main.screenTarget, previewBox, source, Color.White, 0f, Vector2.Zero, effects, 0f);
    }

    private void DrawNPCInBestiary(SpriteBatch sb, Rectangle previewBox) {
        var info = new BestiaryUICollectionInfo
        {
            UnlockState = BestiaryEntryUnlockState.CanShowPortraitOnly_1
        };

        var settings = new EntryIconDrawSettings
        {
            iconbox = previewBox,
            IsPortrait = true
        };

        icon?.Update(info, previewBox, settings);
        icon._npcCache.spriteDirection = -icon._npcCache.spriteDirection;
        if (!NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(icon._npcNetId, out var bestiaryDrawModifiers) || !bestiaryDrawModifiers.PortraitScale.HasValue || bestiaryDrawModifiers.PortraitScale.Value is 1f) {
            icon._npcCache.scale = 1.5f;
        }

        var oldRect = sb.GraphicsDevice.ScissorRectangle;

        sb.GraphicsDevice.ScissorRectangle = previewBox;

        if (icon is not null) {
            try {
                icon?.Draw(info, sb, settings);
            }
            catch {
                icon = null;
            }
        }

        sb.GraphicsDevice.ScissorRectangle = oldRect;
    }

    // 标牌
    private void DrawSignPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
        if (Main.LocalPlayer.sign == -1) {
            return;
        }

        // 画像
        int i = Main.LocalPlayer.sign;
        if (Main.sign[i] is null || !WorldGen.InWorld(Main.sign[i].x, Main.sign[i].y) || !Main.tile[Main.sign[i].x, Main.sign[i].y].HasTile)
            return;

        // Pre
        OnPreSignPortraitDraw?.Invoke(sb, textColor, panel, i);

        var tile = Main.tile[Main.sign[i].x, Main.sign[i].y];
        var position = panel.Location.ToVector2();

        // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);

        var preview = new Rectangle((int)position.X + 17, (int)position.Y + 18, 92, 94);
            
        var tileObjectData = TileObjectData.GetTileData(tile);
        int tileSize = Math.Max(tileObjectData.CoordinateFullHeight, tileObjectData.CoordinateFullWidth) + 20;
        int offset = tileSize / 2;
        float extraZoom = Main.GameZoomTarget - 1f;
        var signCenter = new Point(Main.sign[i].x, Main.sign[i].y).ToWorldCoordinates(16, 16);

        // 根据重力方向调整位置
        var effects = SpriteEffects.None;
        var source = new Rectangle((int)signCenter.X - offset, (int)signCenter.Y - offset - 6, tileSize, tileSize);
        source.Offset((-Main.screenPosition).ToPoint());
        if (Main.LocalPlayer.gravDir is -1) {
            source.Y = Main.screenHeight - source.Y - tileSize;
            effects = SpriteEffects.FlipVertically;
        }
                
        // 根据缩放调整位置
        int inflateValue = (int)(offset * extraZoom);
        source.Inflate(inflateValue, inflateValue);
        var screenOffset = (signCenter - Main.Camera.Center) * extraZoom;
        source.Offset((int)screenOffset.X, (int) (screenOffset.Y * Main.LocalPlayer.gravDir));
                
        sb.Draw(Main.screenTarget, preview, source, Color.White, 0f, Vector2.Zero, effects, 0f);

        // 还原
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        // 名字
        string text = Lang._mapLegendCache.FromTile(Main.Map[Main.sign[i].x, Main.sign[i].y], Main.sign[i].x, Main.sign[i].y);
        Utils.DrawBorderStringFourWay(sb, FontAssets.DeathText.Value, text, 270f + (Main.screenWidth - 800) / 2, 108, textColor, Color.Black, Vector2.Zero, 0.54f);

        // Post
        OnPostSignPortraitDraw?.Invoke(sb, textColor, panel, i);
    }

    internal static void DrawPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
        if (OnPortraitDraw is not null)
            OnPortraitDraw.Invoke(sb, textColor, panel);
    }
}