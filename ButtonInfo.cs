namespace DialogueTweak
{
    internal class ButtonInfo
    {
        internal Func<string> texturePathInternal;
        internal List<int> npcTypes;
        internal Func<string> buttonText;
        internal string iconTexture => texturePathInternal() ?? "";
        internal Action hoverAction;
        internal Func<bool> available;

        internal bool focused;
        internal ButtonInfo(List<int> npcTypes, Func<string> buttonText, Func<string> iconTexture, Action hoverAction, Func<bool> available = null) {
            this.npcTypes = npcTypes ?? new List<int> { NPCID.None };
            this.buttonText = buttonText;
            this.texturePathInternal = iconTexture;
            this.hoverAction = hoverAction;
            if (!Main.dedServ && !ModContent.HasAsset(this.iconTexture) && this.iconTexture != "") {
                DialogueTweak.Instance.Logger.Warn($"Texture path {this.iconTexture} is missing.");
            }
            this.available = available ?? (() => true);
        }
    }
}
