# Dialogue Panel Rework, a mod that makes your NPC dialogue panel better.
Dialogue Panel Rework, aka Dialogue Tweak, is a mod that completely reworks NPC dialogue panel and sign editing panel. Inspired by the mobile version.

[Chinese version | 中文版看这里](README-zhCN.md)

# Mod.Calls
### ReplaceExtraButtonIcon
```"ReplaceExtraButtonIcon", int NPCID, string texture, [Func<bool> replacementAvailable]```Replaces the extra button icon with your custom icon. Read below for specifics on the parameters.

### ReplaceShopButtonIcon
```"ReplaceShopButtonIcon", int NPCID, string texture, [Func<bool> replacementAvailable]```Replaces the shop button icon with your custom icon. Read below for specifics on the parameters.

## Arguments
### 1.) Replacement Type - ```string```
The first argument should be the replacement type you want. For extra button icon use **ReplaceExtraButtonIcon**, and for shop button icon use **ReplaceShopButtonIcon**.

### 2.) NPC ID - ```int```
Your NPC's ID number is needed. Use ```ModContent.NPCType<>()``` to submit your ID.

### 3.) Texture - ```string```
You have to specify the texture that replaces icons. Use your texture's path or use **Head** if you want to replace icon with the NPC's head.

### 4.) Availability - ```Func<bool>```
You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.

## Examples
Here is a complete example of how to custom your shop icon in this mod:
```CSharp
public override void PostSetupContent() {
    Mod dialogueTweak = ModLoader.GetMod("DialogueTweak");
    if (dialogueTweak != null) {
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
    Mod dialogueTweak = ModLoader.GetMod("DialogueTweak");
    if (dialogueTweak != null) {
        dialogueTweakMod.Call(
            "ReplaceShopButtonIcon",
            NPCID.Guide, // NPC ID
            "Head"); // Then the NPC's head texture will be shown
    }
}
```
