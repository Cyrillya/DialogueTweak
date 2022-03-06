# Dialogue Panel Rework, a mod that makes your NPC dialogue panel better.
Dialogue Panel Rework, aka Dialogue Tweak, is a mod that completely reworks NPC dialogue panel and sign editing panel. Inspired by the mobile version.

[Chinese version | 中文版看这里](README-zhCN.md)

# Mod.Calls (Replacements)
### ReplaceExtraButtonIcon
```"ReplaceExtraButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable]```Replaces the extra button icon with your custom icon. Read below for specifics on the parameters.

### ReplaceShopButtonIcon
```"ReplaceShopButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable]```Replaces the shop button icon with your custom icon. Read below for specifics on the parameters.

### ReplacePortrait
```"ReplacePortrait", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable]```Replaces the portrait with your custom icon. Read below for specifics on the parameters.

## Arguments
### 1.) Replacement Type - ```string```
The first argument should be the replacement type you want. For extra button icon use **ReplaceExtraButtonIcon**, for shop button icon use **ReplaceShopButtonIcon**, and for portrait use **ReplacePortrait**.

### 2.) NPC ID - ```int/List<int>```
Your NPC's ID number is needed. Use ```ModContent.NPCType<>()``` to submit your ID. Use **-1** if you want to acess signs.

### 3.) Texture - ```string```
You have to specify the texture that replaces icons. Use your texture's path or use **Head** if you want to replace icon with the NPC's head.

### 4.) Availability - ```Func<bool>```
You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.

## Examples
Here is a complete example of how to custom your shop icon in this mod:
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweakMod.Call(
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
        dialogueTweakMod.Call(
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
        dialogueTweakMod.Call("OnPostPortraitDraw", DrawSomething);
    }
}
private void DrawNPCPortrait(SpriteBatch sb, Color textColor, Rectangle panel) {
    var tex = ModContent.Request<Texture2D>("TheMod/Assets/Something");
    sb.Draw(tex.Value, panel.Location.ToVector2(), Main.DiscoColor);
}
```
