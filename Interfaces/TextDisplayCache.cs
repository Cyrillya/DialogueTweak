namespace DialogueTweak.Interfaces
{
    internal class TextDisplayCache
    {
        private string _originalText;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        public string[] TextLines {
            get;
            private set;
        }

        public int AmountOfLines {
            get;
            private set;
        }

        public float TextAppeared;

        public void PrepareCache(string text) {
            if ((0 | ((Main.screenWidth != _lastScreenWidth) ? 1 : 0) | ((Main.screenHeight != _lastScreenHeight) ? 1 : 0) | ((_originalText != text) ? 1 : 0)) != 0) {
                _lastScreenWidth = Main.screenWidth;
                _lastScreenHeight = Main.screenHeight;
                _originalText = text;
                TextLines = ChatMethods.WordwrapString(Main.npcChatText, FontAssets.MouseText.Value, 362, out int lineAmount);
                AmountOfLines = lineAmount;
                TextAppeared = 0;
            }
        }
    }
}
