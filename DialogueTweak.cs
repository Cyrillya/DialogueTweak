using Terraria.ModLoader;

namespace DialogueTweak;

public partial class DialogueTweak : Mod
{
    internal static DialogueTweak Instance;

    public override void Load() {
        Instance = this;
    }

    public override void Unload() {
        Instance = null;
        Configuration.Instance = null;
    }

    // Test code
    /*
    public override void PostSetupContent() {
	    DialogueTweakHelper.ReplaceButtonIcon(
		    DialogueTweakHelper.ReplacementType.Shop,
		    NPCID.Guide, // NPC ID
		    "Head"); // Then the NPC's head texture will be shown
		    
        DialogueTweakHelper.ReplaceButtonIcon(DialogueTweakHelper.ReplacementType.Extra, NPCID.GoblinTinkerer, () => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow", frame: () => {
            if (Main.LocalPlayer.direction == -1)
                return new Rectangle(0, 0, 80, 80);
            return new Rectangle(0, 0, 30, 30);
        }, customTextOffset: () => 80);
        DialogueTweakHelper.AddButton(
            NPCID.Angler,
            () => "Sandstorm (1 Gold)",
            () => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow",
            () => {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    Main.NewText("这是假的关闭按钮哦");
            },
            () => true,
            () => {
                if (Main.LocalPlayer.direction == -1)
                    return new Rectangle(0, 0, 22, 22);
                return new Rectangle(10, 10, 11, 11);
            },
            () => {
                return 44;
            });
    }
    */
}