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

        public void PrepareCache(string text) {
            if ((0 | ((Main.screenWidth != _lastScreenWidth) ? 1 : 0) | ((Main.screenHeight != _lastScreenHeight) ? 1 : 0) | ((_originalText != text) ? 1 : 0)) != 0) {
                _lastScreenWidth = Main.screenWidth;
                _lastScreenHeight = Main.screenHeight;
                _originalText = text;
                TextLines = WordwrapString(Main.npcChatText, FontAssets.MouseText.Value, 362, out int lineAmount);
                AmountOfLines = lineAmount;
            }
        }


        // 对于中文有优化的换行，自创
        public static string[] WordwrapString(string text, DynamicSpriteFont font, int maxWidth, out int lineAmount) {
            const int maxLines = 27; // 最大行数限制
            string[] array = new string[maxLines];
            int num = 0;
            // 这里用for给list2添加，是大概是为了一个\n的换行
            // list2似乎是精确到了每个单词
            List<string> list = new List<string>(text.Split('\n'));
            List<string> list2 = new List<string>(list[0].Split(' '));
            for (int i = 1; i < list.Count && i < maxLines; i++) {
                //Main.NewText(list[i]);
                list2.Add("\n");
                list2.AddRange(list[i].Split(' '));
            }

            bool flag = true;
            while (list2.Count > 0) {
                string text2 = list2[0];
                string str = " ";
                if (list2.Count == 1)
                    str = "";

                if (text2 == "\n") {
                    array[num++] += text2;
                    flag = true;
                    if (num >= maxLines)
                        break;

                    list2.RemoveAt(0);
                }
                else if (flag) {
                    if (font.MeasureString(text2).X > (float)maxWidth) {
                        string str2 = text2[0].ToString() ?? "";
                        int num2 = 1;
                        // 如果是以中文结尾(text2[num2]为中文)，那么不需要"-"
                        string hyphen = "";
                        if (IsChineseOrSymbols(text2[num2 - 1]) && text2.Length < num2 + 1 && IsChineseOrSymbols(text2[num2 + 1])) {
                            hyphen = "-";
                        }
                        // 添加文本阶段————文字长度还未触行末
                        while (font.MeasureString(str2 + text2[num2] + hyphen).X <= (float)maxWidth) {
                            str2 += text2[num2++];
                            // 如果下一个索引是英文，上一个是中文
                            if (!IsChineseOrSymbols(text2[num2]) && IsChineseOrSymbols(text2[num2 - 1])) {
                                // 获取单词
                                int count = 1;
                                string word = "" + text2[num2];
                                while (!IsChineseOrSymbols(text2[num2 + count])) {
                                    word += text2[num2 + count];
                                    count++;
                                }
                                // 获取单词所占空间，如果超限，直接break，让单词换行
                                if (font.MeasureString(str2 + word).X > (float)maxWidth) {
                                    break;
                                }
                            }
                            // 每次都更新一下，因为选取的字符是不断变动的
                            if (IsChineseOrSymbols(text2[num2 - 1]) && text2.Length < num2 + 1 && IsChineseOrSymbols(text2[num2 + 1])) {
                                hyphen = "-";
                            }
                        }
                        // 换行代码
                        // 如果下一个索引是中文符号，上一个不是
                        if (ChineseSymbols.Exists(t => t == text2[num2]) && !ChineseSymbols.Exists(t => t == text2[num2 - 1])) {
                            // 加上这个符号，并把指针向后移动
                            str2 += text2[num2++];
                            hyphen = "";
                            // 下一个字符是不显示的字符（如换行\n等）
                            while (num2 < text2.Length && font.MeasureString(text2[num2].ToString()).X <= 0) {
                                str2 += text2[num2++];
                            }
                        }

                        str2 += hyphen;
                        array[num++] = str2;
                        // 后面没有字符了
                        if (num2 == text2.Length) {
                            num--; // 拉回来一行
                        }
                        if (num >= maxLines)
                            break;

                        list2.RemoveAt(0); // 清空list2，方便下一行代码的应用
                        if (num2 < text2.Length) list2.Insert(0, text2.Substring(num2)); // 将剩余文本重新放回到list2（待处理文本区）内
                    }
                    else {
                        ref string reference = ref array[num];
                        reference = reference + text2 + str;
                        flag = false;
                        list2.RemoveAt(0);
                    }
                }
                else if (font.MeasureString(array[num] + text2).X > (float)maxWidth) {
                    num++;
                    if (num >= maxLines)
                        break;

                    flag = true;
                }
                else {
                    ref string reference2 = ref array[num];
                    reference2 = reference2 + text2 + str;
                    flag = false;
                    list2.RemoveAt(0);
                }
            }

            lineAmount = num;
            if (lineAmount == maxLines)
                lineAmount--;

            return array;
        }

        // 检测字符是否为中文

        public static readonly List<char> ChineseSymbols = new List<char>() {
            '–', '—', '‘', '’', '“', '”',
            '…', '、', '。', '〈', '〉', '《',
            '》', '「', '」', '『', '』', '【',
            '】', '〔', '〕', '！', '（', '）',
            '，', '．', '：', '；', '？'
        };
        // https://www.qqxiuzi.cn/zh/hanzi-unicode-bianma.php 汉字Unicode编码范围表
        public static bool IsChinese(char a) => (a >= 0x4E00 && a <= 0x9FEF);
        public static bool IsChineseOrSymbols(char a) => IsChinese(a) || ChineseSymbols.Exists(t => t == a);
    }
}
