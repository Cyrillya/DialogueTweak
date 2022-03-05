namespace DialogueTweak.Interfaces.UI.GUIChat
{
    // 专门用于拦截原版NPC对话的ModSystem
    public class GUIChatOverwrite : ModSystem
    {
        public static GUIChat MobileChat = new();

        public override void Load() {
            base.Load();
            On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;

        }

        // 通过调整screenWidth使一切绘制到屏幕之外，NPC对话机制不会被影响
        private void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self) {
            // 确保是处于NPC对话状态（PC版中编辑告示牌什么的也是这个UI）
            MobileChat.GUIDrawInner();
        }
    }
}
