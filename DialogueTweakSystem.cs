namespace DialogueTweak
{
    internal class DialogueTweakSystem : ModSystem
    {
        internal UserInterface UserInterface;
        private UIState UI;

        public override void Load() {
            if (!Main.dedServ) {
                UI = new ChatUI();
                UI.Activate();
                UserInterface = new UserInterface();
                UserInterface.SetState(UI);
            }
        }

        public override void Unload() {
            UI = null;
            UserInterface = null;
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime) {
            _lastUpdateUiGameTime = gameTime;
            UserInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int dialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if (dialogIndex != -1) {
                // 直接替换原版的绘制层
                layers[dialogIndex] = new LegacyGameInterfaceLayer(
                    "DialogueTweak: Reworked Dialog Panel",
                    delegate {
                        if ((Main.npcChatText != "" || Main.LocalPlayer.sign != -1) && !Main.editChest) {
                            UserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    }, InterfaceScaleType.UI);
            }
        }
    }
}
