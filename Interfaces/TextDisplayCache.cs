using ReLogic.Text;
using Terraria.GameContent.UI.Chat;

namespace DialogueTweak.Interfaces
{
    internal class TextDisplayCache
    {
        private string _originalText;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        public TextSnippet[] Snippets {
            get;
            private set;
        }

        public int AmountOfLines {
            get;
            private set;
        }

        public void PrepareCache(string text) {
            if ((0 | ((Main.screenWidth != _lastScreenWidth) ? 1 : 0) | ((Main.screenHeight != _lastScreenHeight) ? 1 : 0) | ((_originalText != text) ? 1 : 0)) != 0) {
                _lastScreenWidth = Main.screenWidth;
                _lastScreenHeight = Main.screenHeight;

                _originalText = text;
                Snippets = WordwrapString(_originalText, FontAssets.MouseText.Value, 362);
                ChatManager.ConvertNormalSnippets(Snippets);

                AmountOfLines = 0;
                foreach (var s in Snippets) {
                    var texts = s.Text.Split('\n');
                    foreach (var t in texts) {
                        AmountOfLines++;
                        ChatUI.TotalLetters += t.Length + 1; // +1换行符
                    }
                    AmountOfLines -= 1;
                }
                AmountOfLines = Math.Min(AmountOfLines, ChatUI.MAX_LINES);
            }
        }

        // 针对textSnippet特殊文本的换行
        public TextSnippet[] WordwrapString(string text, DynamicSpriteFont font, int maxWidth) {
            float workingLineLength = 0f; // 当前行长度
            TextSnippet[] originalSnippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(originalSnippets);
            List<TextSnippet> finalSnippets = new() { new TextSnippet() };

            foreach (var snippet in originalSnippets) {
                if (snippet is PlainTagHandler.PlainSnippet) {
                    string cacheString = ""; // 缓存字符串 - 准备输入的字符
                    for (int i = 0; i < snippet.Text.Length; i++) {
                        GlyphMetrics characterMetrics = font.GetCharacterMetrics(snippet.Text[i]);
                        workingLineLength += font.CharacterSpacing + characterMetrics.KernedWidth;

                        if (workingLineLength > maxWidth && !char.IsWhiteSpace(snippet.Text[i])) {
                            // 如果第一个字符是空格，单词长度小于19（实际上是18因为第一个字符为空格），可以空格换行
                            bool canWrapWord = cacheString.Length > 1 && cacheString.Length < 19;

                            // 找不到空格，或者拆腻子，则强制换行
                            if (!canWrapWord || (i > 0 && CanBreakBetween(snippet.Text[i - 1], snippet.Text[i]))) {
                                finalSnippets.Add(new TextSnippet(cacheString, snippet.Color));
                                finalSnippets.Add(new TextSnippet("\n"));
                                workingLineLength = characterMetrics.KernedWidthOnNewLine;
                                cacheString = "";
                            }
                            // 空格换行
                            else if (canWrapWord) {
                                finalSnippets.Add(new TextSnippet("\n"));
                                finalSnippets.Add(new TextSnippet(cacheString[1..], snippet.Color));
                                workingLineLength = font.MeasureString(cacheString).X;
                                cacheString = "";
                            }
                        }

                        // 这么做可以分割单词，并且使自然分割单词（即不因换行过长强制分割的单词）第一个字符总是空格
                        // 或者是将CJK字符与非CJK字符分割
                        if (cacheString != string.Empty && (char.IsWhiteSpace(snippet.Text[i]) || IsCJK(cacheString[^1]) != IsCJK(snippet.Text[i]))) {
                            finalSnippets.Add(new TextSnippet(cacheString, snippet.Color));
                            cacheString = "";
                        }

                        // 原有换行则将当前行长度重置
                        if (snippet.Text[i] is '\n') {
                            workingLineLength = 0;
                        }

                        cacheString += snippet.Text[i];
                    }
                    finalSnippets.Add(new TextSnippet(cacheString, snippet.Color));
                }
                else {
                    float length = snippet.GetStringLength(font);
                    workingLineLength += length;
                    // 超了 - 换行再添加，注意起始长度
                    if (workingLineLength > maxWidth) {
                        workingLineLength = length;
                        finalSnippets.Add(new TextSnippet("\n"));
                    }
                    finalSnippets.Add(snippet);
                }
            }

            return finalSnippets.ToArray();
        }

        // https://unicode-table.com/cn/blocks/cjk-unified-ideographs/ 中日韩统一表意文字
        // https://unicode-table.com/cn/blocks/cjk-symbols-and-punctuation/ 中日韩符号和标点
        public static bool IsCJK(char a) {
            return (a >= 0x4E00 && a <= 0x9FFF) || (a >= 0x3000 && a <= 0x303F);
        }

        internal static bool CanBreakBetween(char previousChar, char nextChar) {
            if (IsCJK(previousChar) || IsCJK(nextChar))
                return true;

            return false;
        }
    }
}
