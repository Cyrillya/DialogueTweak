namespace DialogueTweak;

internal class DialogueTweakSystem : ModSystem
{
    private UserInterface _userInterface;
    private UIState _ui;
    private bool _fancyUIDrawing;

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
            if (_fancyUIDrawing && !Configuration.Instance.VanillaUI) {
                return;
            }
            orig.Invoke(self);
        };
    }

    public override void Unload() {
        _ui = null;
        _userInterface = null;
    }

    private GameTime _lastUpdateUiGameTime;
    public override void UpdateUI(GameTime gameTime) {
        if (Configuration.Instance.VanillaUI) return;
        _lastUpdateUiGameTime = gameTime;
        _userInterface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        int dialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
        if (dialogIndex != -1) {
            layers[dialogIndex].Active = Configuration.Instance.VanillaUI;
            layers.Insert(dialogIndex, new LegacyGameInterfaceLayer(
                "DialogueTweak: Reworked Dialog Panel",
                delegate {
                    if (!Configuration.Instance.VanillaUI && (Main.npcChatText != "" || Main.LocalPlayer.sign != -1) && !Main.editChest) {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }
    }
}