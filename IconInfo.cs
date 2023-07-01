namespace DialogueTweak
{
    internal enum IconType
    {
        Happiness,
        Back,
        Shop,
        Extra
    }

    internal class IconInfo
    {
        internal Func<string> textureInternal;
        internal IconType iconType;
        internal List<int> npcTypes;
        internal string texture => textureInternal() ?? "";
        internal Func<bool> available;
        internal Func<Rectangle> frame;
        internal IconInfo(IconType iconType, List<int> npcTypes, Func<string> texture, Func<bool> available = null, Func<Rectangle> frame = null) {
            this.iconType = iconType;
            this.npcTypes = npcTypes ?? new List<int> { NPCID.None };
            textureInternal = texture;
            if (!Main.dedServ && !ModContent.HasAsset(this.texture) && this.texture != "" &&
                ((iconType is IconType.Shop or IconType.Extra && this.texture is not "Head") || // 是Shop, Extra图标，但不是Head而且是给NPC用的
                 iconType is IconType.Happiness or IconType.Back)) { // 是Happiness, Back
                DialogueTweak.Instance.Logger.Warn($"Texture path {this.texture} is missing.");
            }
            this.available = available ?? (() => true);
            this.frame = frame;
        }
    }
}
