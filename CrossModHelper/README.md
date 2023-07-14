<h1 align="center">Cross-mod Support</h1>

<div align="center">

English | [简体中文](README.zh.md)

</div>

## The Helper Class

Dialogue Panel Rework provides a helper class for modders to easily add their support. Just copy and paste the file [DialogueTweakHelper.cs](DialogueTweakHelper.cs) to your mod's source code and you can use the helper class.

The helper class includes some methods for you to add buttons and change button icons. You don't have to use `Mod.Call`, which lacks documentation and parameter hints.

The helper class itself includes documentation, so you can easily use it without being confused.

### Examples

The following example adds a button to the angler using the close texture of this mod, which reads "Close". When you click on it, a message will be shown.

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.AddButton(
	    NPCID.Angler,
	    () => Language.GetTextValue("LegacyInterface.52"), // "Close"
	    "DialogueTweak/Interfaces/Assets/Button_Back", // Directly referencing this mod's texture.
	    () => {
		    if (Main.mouseLeft && Main.mouseLeftRelease)
			    Main.NewText("This is the fake closing button :P");
	    });
}
```

Here is a complete example of how to custom your shop icon in this mod:

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.ReplaceButtonIcon(
	    DialogueTweakHelper.ReplacementType.Shop,
	    ModContent.NPCType<NPCs.MyNPC>(), // NPC ID
	    "TheMod/Assets/NPCShopIcon", // The texture's path
	    () => Main.LocalPlayer.direction >= 0); // Shown only when the player faces right.
}
```

If you want to replace the guide's shop icon (aka "help" button icon) with the NPC's head, use this:

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.ReplaceButtonIcon(
	    DialogueTweakHelper.ReplacementType.Shop,
	    NPCID.Guide, // NPC ID
	    "Head"); // Then the NPC's head texture will be shown
}
```


## Mod.Call

`Mod.Call` is very useful, and is a very easy way for mods to communicate with each other.

For a simple guide on `Mod.Call`, check [this guide](https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate) written by tModLoader team.

You can use `Mod.Call` to add support for the Dialogue Panel Rework mod. But the recommended approach is to use the helper class provided by this mod.

You must use `Mod.Call` if you want to access the drawing of the panel.

### Use Mod.Call for Drawing


#### OnPostPortraitDraw

```"OnPostPortraitDraw", Action<SpriteBatch, Color, Rectangle> drawingAction```Draw your own things after portrait drawing.

#### OnPreNPCPortraitDraw

```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```Draw your own things before NPC portrait drawing.

#### OnPostNPCPortraitDraw

```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```Draw your own things after NPC portrait drawing.

#### OnPreSignPortraitDraw

```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```Draw your own things before sign portrait drawing.

#### OnPostSignPortraitDraw

```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```Draw your own things after sign portrait drawing.

### Arguments

#### 1.) Drawing Hook Type - ```string```

The drawing hook type you want. Read above for a list of drawing hook types.

#### 2.) Drawing Action - ```Action<SpriteBatch, Color, Rectangle, int/NPC>```

Your drawing action lands here.

**```SpriteBatch``` - The SpriteBatch instance for you to draw your things.**

**```Color``` - The text color of the NPC/sign's name.**

**```Rectangle``` - The rectangle of the whole panel.**

**```int``` - The sign's index, and ```NPC``` - The instance of the NPC**

### Examples

Here is a complete example of how to draw something on the top left of the panel with disco color:

```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweak.Call("OnPostPortraitDraw", DrawSomething);
    }
}
private void DrawSomething(SpriteBatch sb, Color textColor, Rectangle panel) {
    var tex = ModContent.Request<Texture2D>("TheMod/Assets/Something");
    sb.Draw(tex.Value, panel.Location.ToVector2(), Main.DiscoColor);
}
```