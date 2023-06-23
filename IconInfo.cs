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
        internal IconType iconType;
        internal List<int> npcTypes;
        internal string texture;
        internal Func<bool> available;
        internal Func<Rectangle> frame;
        internal IconInfo(IconType iconType, List<int> npcTypes, string texture, Func<bool> available = null, Func<Rectangle> frame = null) {
            this.iconType = iconType;
            this.npcTypes = npcTypes ?? new List<int> { NPCID.None };
            this.texture = texture ?? "";
            if (!Main.dedServ && !ModContent.HasAsset(this.texture) && this.texture != "" &&
                (((iconType == IconType.Shop || iconType == IconType.Extra) && this.texture != "Head") || // 是Shop, Extra图标，但不是Head而且是给NPC用的
                 (iconType == IconType.Happiness || iconType == IconType.Back))) { // 是Happiness, Back
                DialogueTweak.Instance.Logger.Warn($"Texture path {this.texture} is missing.");
                this.texture = "";
            }
            this.available = available ?? (() => true);
            this.frame = frame;
        }
    }
}
