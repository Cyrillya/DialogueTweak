# Dialogue Panel Rework, a mod that makes your NPC dialogue panel better.

Dialogue Panel Rework, aka Dialogue Tweak, is a mod that completely reworks NPC dialogue panel and sign editing panel. Inspired by the mobile version.

# Compiling

**NOTE**: You should not use the compilation of tModLoader, but you should use the compilation function of the code IDE (such as Visual Studio, Rider) to compile this mod, because this mod contains Nuget package

[Chinese version | 中文版看这里](README-zhCN.md)

# Mod.Calls (Replacements)

### ReplaceExtraButtonIcon

```"ReplaceExtraButtonIcon", int/List<int> NPCIDs, string/Func<string> texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```Replaces the extra button icon with your custom icon. Read below for specifics on the parameters.

### ReplaceShopButtonIcon

```"ReplaceShopButtonIcon", int/List<int> NPCIDs, string/Func<string> texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```Replaces the shop button icon with your custom icon. Read below for specifics on the parameters.

### ReplaceHappinessButtonIcon

```"ReplaceHappinessButtonIcon", int/List<int> NPCIDs, string/Func<string> texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```Replaces the happiness button icon with your custom icon. Read below for specifics on the parameters.

### ReplaceBackButtonIcon

```"ReplaceBackButtonIcon", int/List<int> NPCIDs, string/Func<string> texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```Replaces the back button icon with your custom icon. Read below for specifics on the parameters.

## Arguments

### 1.) Replacement Type - ```string```

The first argument should be the replacement type you want. The type table is listed above.

### 2.) NPC IDs - ```int/List<int>```

Your NPC's ID number is needed. Use ```ModContent.NPCType<>()``` to submit your ID. Use **-1** if you want to acess signs.

### 3.) Texture - ```string/Func<string>```

You have to specify the texture that replaces icons. Use your texture's path. Use **Head** for button icons if you want to replace icon with the NPC's head.

### 4.) Availability - ```Func<bool>```

You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.

### 5.) Frame - ```Func<Rectangle>```

You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.

## Examples

Here is a complete example of how to custom your shop icon in this mod:

```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweak.Call(
            "ReplaceShopButtonIcon",
            ModContent.NPCType<NPCs.MyNPC>(), // NPC ID
            "TheMod/Assets/NPCShopIcon", // The texture's path
            (Func<bool>)(() => Main.LocalPlayer.direction >= 0)); // Shown only when the player faces right.
    }
}
```

If you want to replace the guide's shop icon (aka "help" button icon) with the NPC's head, use this:

```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweak.Call(
            "ReplaceShopButtonIcon",
            NPCID.Guide, // NPC ID
            "Head"); // Then the NPC's head texture will be shown
    }
}
```

# Mod.Calls (Drawings)

### OnPostPortraitDraw

```"OnPostPortraitDraw", Action<SpriteBatch, Color, Rectangle> drawingAction```Draw your own things after portrait drawing.

### OnPreNPCPortraitDraw

```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```Draw your own things before NPC portrait drawing.

### OnPostNPCPortraitDraw

```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```Draw your own things after NPC portrait drawing.

### OnPreSignPortraitDraw

```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```Draw your own things before sign portrait drawing.

### OnPostSignPortraitDraw

```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```Draw your own things after sign portrait drawing.

## Arguments

### 1.) Drawing Hook Type - ```string```

The drawing hook type you want. Read above for a list of drawing hook types.

### 2.) Drawing Action - ```Action<SpriteBatch, Color, Rectangle, int/NPC>```

Your drawing action lands here.

**```SpriteBatch``` - The SpriteBatch instance for you to draw your things.**

**```Color``` - The text color of the NPC/sign's name.**

**```Rectangle``` - The rectangle of the whole panel.**

**```int``` - The sign's index, and ```NPC``` - The instance of the NPC**

## Examples

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

# Mod.Calls (Button)

There are only one Mod.Call for adding buttons now. I think that's enough though.

### AddButton

```"AddButton", int/List<int> NPCIDs, Func<string> buttonText, string texture, Action hoverAction, [Func<bool> replacementAvailable]```Adds a new button.

## Arguments

### 1.) Button Adding Type - ```string```

Just use "AddButton".

### 2.) NPC IDs - ```int/List<int>```

Your NPC's ID number is needed. Use ```ModContent.NPCType<>()``` to submit your ID. You can also tweak vanilla NPCs by using vanilla NPC ID.

### 3.) Button Text - ```Func<string>```

This is the text which will be shown in the button. It is ```Func<string>``` so you can use Language.GetTextValue or something else.

### 4.) Icon Texture - ```string```

You have to specify the icon texture of the button. Use your texture's path. If you use "" aka nothing, no icon will be shown.

### 5.) Hover Action - ```Action```

The action that will be called when the client hovers over the button. Use this to define the behavior when the button is pressed.

### 6.) Availability - ```Func<bool>```

You can decide if your button should be shown.

## Examples

The following example adds a button using the close texture of this mod reads "Close". When you click on it, a message will be shown.

```CSharp
public override void PostSetupContent() {
	if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
		dialogueTweak.Call(
			"AddButton",
			NPCID.Angler,
			(Func<string>)(() => Language.GetTextValue("LegacyInterface.52")), // "Close"
			"DialogueTweak/Interfaces/Assets/Button_Back", // Directly referencing this mod's texture.
			(Action)(() => {
				if (Main.mouseLeft)
                    Main.NewText("This is the fake closing button :P");
			}));
	}
}
```
