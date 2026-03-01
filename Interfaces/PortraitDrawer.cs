using System;
using System.Collections.Generic;

using DialogueTweak.Port;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReLogic.Content;

using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DialogueTweak.Interfaces;

internal class PortraitDrawer : ModSystem
{
    private static bool NewVersionPortrait => Main.LocalPlayer.sign == -1 && Configuration.Instance.PortraitDrawStyle is Configuration.PortraitStyle.Portrait or Configuration.PortraitStyle.Profile;

    private static NPC _portraitDummy = new NPC();
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
        if (Main.LocalPlayer.talkNPC < 0 || Main.LocalPlayer.sign != -1 || Main.npc[Main.LocalPlayer.talkNPC] is null ||
            !Main.npc[Main.LocalPlayer.talkNPC].active) {
            return;
        }

        var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

        // Pre
        OnPreNPCPortraitDraw?.Invoke(sb, textColor, panel, talkNPC);

        var position = panel.Location.ToVector2();

        // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null,
            Main.UIScaleMatrix);

        var previewBox = new Rectangle((int) position.X + 16, (int) position.Y + 16, 96, 96);

        if (talkNPC.active) {
            bool screenTargetUnavailable = Main.screenTarget is null || Main.screenTarget.IsDisposed ||
                                           !Lighting.NotRetro || Main.WaveQuality <= 0;
            if (Configuration.Instance.PortraitDrawStyle is Configuration.PortraitStyle.Portrait) {
                DrawNPCPortraitDetailed(sb, talkNPC, previewBox);
            }
            else if (Configuration.Instance.PortraitDrawStyle is Configuration.PortraitStyle.Profile
                    or Configuration.PortraitStyle.Retro || screenTargetUnavailable) {
                DrawNPCInBestiary(sb, talkNPC, previewBox);
            }
            else {
                DrawNPCRenderedInWorld(sb, talkNPC, previewBox);
            }
        }

        // 还原
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None,
            RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        // Post
        OnPostNPCPortraitDraw?.Invoke(sb, textColor, panel, talkNPC);
    }

    private void DrawNPCPortraitDetailed(SpriteBatch sb, NPC talkNPC, Rectangle previewBox) {
        if (!NewSourceCode.NPCPortraits.TryGetValue(talkNPC.type, out var value))
            return;

        value.GetDrawData(out var texture, out var drawFrame);
        if (texture == null)
            return;
        sb.Draw(texture, previewBox.Center.ToVector2(), null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
    }

    private void DrawNPCRenderedInWorld(SpriteBatch sb, NPC talkNPC, Rectangle previewBox) {
        int npcSize = Math.Max(talkNPC.height, talkNPC.width) + 20;
        int offset = npcSize / 2;
        float extraZoom = Main.GameViewMatrix.Zoom.X - 1f;

        // 根据重力方向调整位置
        var effects = SpriteEffects.None;
        var source = new Rectangle((int) talkNPC.Center.X - offset, (int) talkNPC.Center.Y - offset - 6, npcSize,
            npcSize);
        source.Offset((-Main.screenPosition).ToPoint());
        if (Main.LocalPlayer.gravDir is -1) {
            source.Y = Main.screenHeight - source.Y - npcSize;
            effects = SpriteEffects.FlipVertically;
        }

        // 根据缩放调整位置
        int inflateValue = (int) (offset * extraZoom);
        source.Inflate(inflateValue, inflateValue);
        var screenOffset = (talkNPC.Center - Main.Camera.Center) * extraZoom;
        source.Offset((int) screenOffset.X, (int) (screenOffset.Y * Main.LocalPlayer.gravDir));
        var uiZoomOffset = -Vector2.One * Main.ScreenSize.ToVector2() * 0.5f * extraZoom * (Main.UIScale - 1f);
        if (Main.LocalPlayer.gravDir is -1f) uiZoomOffset.Y = -1f * Main.screenHeight * 0.5f * 1 * (Main.UIScale - 1f);
        if (Main.LocalPlayer.gravDir is 1f)
            source.Offset((int) uiZoomOffset.X, (int) uiZoomOffset.Y);
        else
            source.Offset((int) uiZoomOffset.X, (int) (-uiZoomOffset.Y * (Main.GameZoomTarget + 1f)));

        sb.Draw(Main.screenTarget, previewBox, source, Color.White, 0f, Vector2.Zero, effects, 0f);
    }

    private void DrawNPCInBestiary(SpriteBatch sb, NPC talkNPC, Rectangle previewBox)
    {
        var center = previewBox.Center.ToVector2();
        if (Configuration.Instance.PortraitDrawStyle is Configuration.PortraitStyle.Profile)
        {
            NPC dummy = _portraitDummy;
            dummy.SetDefaults(talkNPC.type);
            dummy.whoAmI = talkNPC.whoAmI;
            dummy.GivenName = talkNPC.GivenName;
            dummy.townNpcVariationIndex = talkNPC.townNpcVariationIndex;
            dummy.FindFrame();
            dummy.direction = dummy.spriteDirection = 1;
            int num14 = 16;
            dummy.scale = 3f;
            int num15 = -dummy.width / 2;
            if (NPCID.Sets.IsTownPet[dummy.type])
            {
                num14 = -20;
                dummy.scale = 3f;
                int num16 = 96;
                num16 -= talkNPC.frame.Width;
                num16 /= 2;
                num16 /= 6;
                num16 *= 6;
                num15 = -36 + num16;
                dummy.direction = dummy.spriteDirection = -1;
            }
            Dictionary<int, Vector2> nPCPortraitsCloseUpOffsets = NewSourceCode.NPCPortraitsCloseUpOffsets;
            dummy.position = center + new Vector2(num15, 48 + num14);
            Vector2 value = Vector2.Zero;
            if (nPCPortraitsCloseUpOffsets.TryGetValue(talkNPC.type, out value))
            {
                dummy.position += value;
            }
            dummy.IsABestiaryIconDummy = true;
            sb.End();
            Rectangle scissorRectangle = sb.GraphicsDevice.ScissorRectangle;
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, ScissorState, null,
                Main.UIScaleMatrix);
            sb.GraphicsDevice.ScissorRectangle = previewBox;
            Main.instance.DrawNPCDirect(sb, dummy, behindTiles: false, Vector2.Zero);
            sb.End();
            sb.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null,
                Main.UIScaleMatrix);
        }
        if (Configuration.Instance.PortraitDrawStyle is Configuration.PortraitStyle.Retro)
        {
            NPC dummy = _portraitDummy;
            dummy.SetDefaults(talkNPC.type);
            dummy.whoAmI = talkNPC.whoAmI;
            dummy.GivenName = talkNPC.GivenName;
            dummy.townNpcVariationIndex = talkNPC.townNpcVariationIndex;
            dummy.FindFrame();
            dummy.direction = dummy.spriteDirection = 1;
            if (NPCID.Sets.IsTownPet[dummy.type])
            {
                dummy.direction = dummy.spriteDirection = -1;
            }
            int num17 = -dummy.height;
            dummy.scale = 2f;
            int num18 = -dummy.width / 2;
            Dictionary<int, Vector2> nPCPortraitsFullBodyRetroOffsets = NewSourceCode.NPCPortraitsFullBodyRetroOffsets;
            dummy.position = center + new Vector2(num18, 48 + num17);
            Vector2 value2 = Vector2.Zero;
            if (nPCPortraitsFullBodyRetroOffsets.TryGetValue(talkNPC.type, out value2))
            {
                dummy.position += value2;
            }
            dummy.IsABestiaryIconDummy = true;
            Main.instance.DrawNPCDirect(sb, dummy, behindTiles: false, Vector2.Zero);
        }
    }

    // 标牌
    private void DrawSignPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
        if (Main.LocalPlayer.sign == -1) {
            return;
        }

        // 画像
        int i = Main.LocalPlayer.sign;
        if (Main.sign[i] is null || !WorldGen.InWorld(Main.sign[i].x, Main.sign[i].y) ||
            !Main.tile[Main.sign[i].x, Main.sign[i].y].HasTile)
            return;

        // Pre
        OnPreSignPortraitDraw?.Invoke(sb, textColor, panel, i);

        var tile = Main.tile[Main.sign[i].x, Main.sign[i].y];
        var position = panel.Location.ToVector2();

        // 重新开启spriteBatch，以去掉不明觉厉的UI绘制的一层模糊滤镜
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null,
            Main.UIScaleMatrix);

        var previewBox = new Rectangle((int) position.X + 16, (int) position.Y + 16, 96, 96);

        var tileObjectData = TileObjectData.GetTileData(tile);
        int tileSize = Math.Max(tileObjectData.CoordinateFullHeight, tileObjectData.CoordinateFullWidth) + 20;
        int offset = tileSize / 2;
        float extraZoom = Main.GameZoomTarget - 1f;
        var signCenter = new Point(Main.sign[i].x, Main.sign[i].y).ToWorldCoordinates(16, 16);

        // 根据重力方向调整位置
        var effects = SpriteEffects.None;
        var source = new Rectangle((int) signCenter.X - offset, (int) signCenter.Y - offset - 6, tileSize, tileSize);
        source.Offset((-Main.screenPosition).ToPoint());
        if (Main.LocalPlayer.gravDir is -1) {
            source.Y = Main.screenHeight - source.Y - tileSize;
            effects = SpriteEffects.FlipVertically;
        }

        // 根据缩放调整位置
        int inflateValue = (int) (offset * extraZoom);
        source.Inflate(inflateValue, inflateValue);
        var screenOffset = (signCenter - Main.Camera.Center) * extraZoom;
        source.Offset((int) screenOffset.X, (int) (screenOffset.Y * Main.LocalPlayer.gravDir));

        sb.Draw(Main.screenTarget, previewBox, source, Color.White, 0f, Vector2.Zero, effects, 0f);

        // 还原
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None,
            RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        // Post
        OnPostSignPortraitDraw?.Invoke(sb, textColor, panel, i);
    }

    private static float npcChatPortraitFrameCounter;

    public static void DoNPCPortraitHop()
    {
        npcChatPortraitFrameCounter = 0;
        if (!Configuration.Instance.PortraitAnimation)
            npcChatPortraitFrameCounter = 2.01f;
    }

    internal static void DrawPortrait(SpriteBatch sb, Color textColor, Rectangle panel)
    {
        int offsetY = 0;
        npcChatPortraitFrameCounter += (float) Main._drawInterfaceGameTime.ElapsedGameTime.TotalSeconds;
        if (npcChatPortraitFrameCounter <= 2f)
        {
            double num9 = 80.0;
            float num10 = 0.25f;
            float num11 = (float) EaseOutBounce(Utils.Clamp((int)(npcChatPortraitFrameCounter * 60f), 0.0, num9) / num9);
            offsetY = (int)(-56f * num10 * (1f - num11));
        }

        panel.Y += offsetY;

        var position = panel.Location.ToVector2();
        var previewBox = new Rectangle((int) position.X + 14, (int) position.Y + 14, 100, 100);

        Main.spriteBatch.Draw(ModAsset.PortraitPanel_Overlay.Value, previewBox.Location.ToVector2(), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        // 如果是1.4.5版本肖像，因为有些肖像会出框，所以把框放在前面绘制
        if (NewVersionPortrait)
            Main.spriteBatch.Draw(ModAsset.PortraitPanel_Front.Value, previewBox.Location.ToVector2(), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        if (OnPortraitDraw is not null)
            OnPortraitDraw.Invoke(sb, textColor, panel);

        // 如果是1.4.5版本肖像，因为有些肖像会出框，所以把框放在前面绘制，这里不绘制
        if (!NewVersionPortrait)
            Main.spriteBatch.Draw(ModAsset.PortraitPanel_Front.Value, previewBox.Location.ToVector2(), null, Color.White * 0.92f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    public static double EaseOutBounce(double x)
    {
        return BounceEaseOut(x, 4, 2.0);
    }

    private static double BounceEaseOut(double t, int bounces, double elasticity)
    {
        double num = (double) bounces * Math.PI;
        double num2 = Math.Pow(1.0 - t, elasticity);
        double num3 = Math.Abs(Math.Sin(t * num));
        return 1.0 - num2 * num3;
    }

    private static RasterizerState ScissorState = new RasterizerState
    {
        CullMode = CullMode.None,
        ScissorTestEnable = true
    };
}