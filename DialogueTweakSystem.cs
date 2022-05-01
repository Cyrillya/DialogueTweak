namespace DialogueTweak
{
    internal class DialogueTweakSystem : ModSystem
    {
        internal static bool cursorAtTextPanel;
        internal static string prevText;
        internal static float letterAppeared;

        public override void Load() {
            On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
        }

        // 通过调整screenWidth使一切绘制到屏幕之外，NPC对话机制不会被影响
        private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
            // 确保是处于NPC对话状态（PC版中编辑告示牌什么的也是这个UI）
            GUIChatDraw.GUIDrawInner();
        }

        // Text scrolling
        public override void UpdateUI(GameTime gameTime) {
            if (Main.npcChatText != prevText) {
                letterAppeared = 0;
            }
            prevText = Main.npcChatText;
            if (!Main.npc.IndexInRange(Main.LocalPlayer.talkNPC) || Main.npc[Main.LocalPlayer.talkNPC] is null || !Main.npc[Main.LocalPlayer.talkNPC].active) {
                return;
            }
            if (Main.LocalPlayer.sign > -1) {
                letterAppeared = 1145141919; // 标牌没有缓慢出现机制
                return;
            }
            if (letterAppeared < Main.npcChatText.Length + 1) {
                float speakingRateMultipiler = GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive ? 1.2f : 2f;
                if (cursorAtTextPanel) {
                    speakingRateMultipiler *= 1.2f;
                    if (Main.mouseLeft) {
                        speakingRateMultipiler *= 3f; // 快速吟唱
                    }
                }
                letterAppeared += ChatMethods.HandleSpeakingRate(Main.npc[Main.LocalPlayer.talkNPC].type) * speakingRateMultipiler;
            }
            cursorAtTextPanel = false;
        }
    }
}
