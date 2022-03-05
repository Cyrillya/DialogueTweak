namespace DialogueTweak
{
    internal enum IconType
    {
        Shop,
        Extra
    }

    internal class IconInfo
    {
        internal IconType iconType;
        internal int npcType;
        internal string texture;
        internal Func<bool> available;
        internal IconInfo(IconType iconType, int npcType, string texture, Func<bool> available = null) {
            this.iconType = iconType;
            this.npcType = npcType;
            this.texture = texture ?? "";
            if (!Main.dedServ && !ModContent.HasAsset(this.texture) && this.texture != "" && this.texture != "Head") {
                DialogueTweak.instance.Logger.Warn($"Texture path {this.texture} is missing.");
                this.texture = "";
            }
            this.available = available ?? (() => true);
        }
    }
}
