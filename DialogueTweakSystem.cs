using System;
using System.Collections.Generic;
using System.Diagnostics;

using DialogueTweak.Interfaces;

using Microsoft.Xna.Framework;

using MonoMod.Cil;

using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace DialogueTweak;

internal class DialogueTweakSystem : ModSystem
{
    internal static Dictionary<int, Func<bool>> ReworkDisableConditions = new ();
    private UserInterface _userInterface;
    private UIState _ui;
    private bool _fancyUIDrawing;

    public static Stopwatch RefreshRateStopwatch = new();

    /// <summary>
    /// 刷新率因子，用于高帧率下的动画支持，保持总时长一致
    /// </summary>
    public static float CurrentRefreshRateFactor = 1f;

    private bool IsReworkPanelDisabled => Main.LocalPlayer.talkNPC >= 0 && DialoguePanelEnabled &&
                                         Main.npc.IndexInRange(Main.LocalPlayer.talkNPC) &&
                                         ReworkDisableConditions.TryGetValue(Main.npc[Main.LocalPlayer.talkNPC].type,
                                             out var disableCondition) && disableCondition.Invoke();

    public override void Load() {
        if (!Main.dedServ) {
            _ui = new ChatUI();
            _ui.Activate();
            _userInterface = new UserInterface();
            _userInterface.SetState(_ui);
        }

        // 标牌正在编辑时，原版对话框会在IngameFancyUI.Draw中被绘制，这里移除这个绘制
        On_IngameFancyUI.Draw += (orig, batch, time) => {
            _fancyUIDrawing = true;
            bool result = orig.Invoke(batch, time);
            _fancyUIDrawing = false;
            return result;
        };

        On_Main.GUIChatDraw += (orig, self) => {
            // do vanilla logic if rework panel is disabled via mod.call
            if (IsReworkPanelDisabled) {
                orig.Invoke(self);
                return;
            }

            if (_fancyUIDrawing && !Configuration.Instance.VanillaUI) {
                return;
            }

            orig.Invoke(self);
        };

        // 使用IL而不是On插入代码，这样的话如果哪个Mod把DoDraw炸了，报错里不会显示我们的信息，不会有人找上门来
        // 原先的实现是在ModifyInterfaceLayers里执行下面的代码，但是那个方法在游戏主界面不会运行，配置中心的界面使用UpdateHighFps就会出问题
        IL_Main.DoDraw += IL_EmitRefreshRate;
    }

    public override void Unload() {
        _ui = null;
        _userInterface = null;
    }

    private GameTime _lastUpdateUiGameTime;

    public override void UpdateUI(GameTime gameTime) {
        if (Configuration.Instance.VanillaUI || IsReworkPanelDisabled) return;
        _lastUpdateUiGameTime = gameTime;
        _userInterface.Update(gameTime);
    }

    private static bool DialoguePanelEnabled =>
        (Main.npcChatText != "" || Main.LocalPlayer.sign != -1) && !Main.editChest;

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        int dialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
        if (dialogIndex != -1) {
            layers[dialogIndex].Active &= Configuration.Instance.VanillaUI || IsReworkPanelDisabled;
            layers.Insert(dialogIndex + 1, new LegacyGameInterfaceLayer(
                "DialogueTweak: Panel Style Toggle Button",
                delegate {
                    var position = ChatUI.PanelPosition;
                    position.X += TextureAssets.ChatBack.Width() + 34;
                    DrawingHelper.DrawGUISwapButton(position);
                    return true;
                }, InterfaceScaleType.UI) {
                Active = Configuration.Instance.VanillaUI && DialoguePanelEnabled &&
                         Configuration.Instance.ShowSwapButton && !IsReworkPanelDisabled
            });
            layers.Insert(dialogIndex, new LegacyGameInterfaceLayer(
                "DialogueTweak: Reworked Dialog Panel",
                delegate {
                    _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    return true;
                }, InterfaceScaleType.UI) {
                Active = !Configuration.Instance.VanillaUI && DialoguePanelEnabled && !IsReworkPanelDisabled
            });
        }
    }

    static void IL_EmitRefreshRate(ILContext il)
    {
        var c = new ILCursor(il);

        c.EmitDelegate(() => { CurrentRefreshRateFactor = GetRefreshRateFactor(RefreshRateStopwatch); });
    }

    /// <summary>
    /// 获取刷新率因子，可以乘在AnimationTimer的缓动上，来根据帧率实时调节动画速度，实现高帧率下的丝滑动画。
    /// 获取到的值为【60 / 当前帧速率】，也就是说若fps为120，则返回值为0.5，若fps为30，则返回值为2。
    /// </summary>
    /// <param name="timer">用于统计两次操作间用时的计时器</param>
    /// <param name="restartTimer">是否重设计时器</param>
    /// <returns>刷新率因子</returns>
    public static float GetRefreshRateFactor(Stopwatch timer, bool restartTimer = true)
    {
        int elapsedMilliseconds = 0;
        float factor = 1f;
        float msPerTick = 1000f / 60f; // 一帧有多少毫秒
        if (!timer.IsRunning && restartTimer)
        {
            timer.Start();
        }
        else
        {
            factor = timer.ElapsedMilliseconds / msPerTick;
            if (restartTimer)
                timer.Restart();
        }

        return factor;
    }
}