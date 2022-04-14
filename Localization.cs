namespace DialogueTweak
{
    // There are not many texts so just make localizations here
    internal class Localization : ModSystem
    {
        public override void PostSetupContent() {
            AddTranslation(Mod, "Hate", "Hassen", "Odio", "Je déteste", "Odio", "Ненавидеть", "讨厌", "Ódio", "Nienawiść");
            AddTranslation(Mod, "Dislike", "Nicht mögen", "Dislike", "N'aime pas", "No me gusta", "Не нравится", "不喜", "Não gostar", "Nie lubię");
            AddTranslation(Mod, "Like", "Gefällt", "Come", "J'aime bien", "Como", "нравиться", "喜欢", "Como", "Lubię");
            AddTranslation(Mod, "Love", "Lieben", "Amore", "J'aime", "Me gusta", "любить", "喜爱", "Amor", "Love");
        }

        //English = 1,
        //German = 2,
        //Italian = 3,
        //French = 4,
        //Spanish = 5,
        //Russian = 6,
        //Chinese = 7,
        //Portuguese = 8,
        //Polish = 9,
        //Unknown = 9999
        private static void AddTranslation(Mod Mod, string english, string german, string italian, string french, string spanish, string russian, string chinese, string portuguese, string polish) {
            ModTranslation translation = LocalizationLoader.CreateTranslation(Mod, english);
            translation.AddTranslation(1, english);
            translation.AddTranslation(2, german);
            translation.AddTranslation(3, italian);
            translation.AddTranslation(4, french);
            translation.AddTranslation(5, spanish);
            translation.AddTranslation(6, russian);
            translation.AddTranslation(7, chinese);
            translation.AddTranslation(8, portuguese);
            translation.AddTranslation(9, polish);
            LocalizationLoader.AddTranslation(translation);
        }
    }
}