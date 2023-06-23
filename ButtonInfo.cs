namespace DialogueTweak
{
    internal class ButtonInfo
    {
        internal List<int> npcTypes;
        internal Func<string> buttonText;
        internal string iconTexture;
        internal Action hoverAction;
        internal Func<bool> available;

        internal bool focused;
        internal Asset<Texture2D> texture;
        internal ButtonInfo(List<int> npcTypes, Func<string> buttonText, string iconTexture, Action hoverAction, Func<bool> available = null) {
            this.npcTypes = npcTypes ?? new List<int> { NPCID.None };
            this.buttonText = buttonText;
            this.iconTexture = iconTexture ?? "";
            this.hoverAction = hoverAction;
            if (!Main.dedServ && !ModContent.HasAsset(this.iconTexture) && this.iconTexture != "") {
                DialogueTweak.Instance.Logger.Warn($"Texture path {this.iconTexture} is missing.");
                this.iconTexture = "";
            }
            this.available = available ?? (() => true);
        }
    }
}
