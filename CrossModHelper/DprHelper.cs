using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

// Change the namespace to yours!
namespace DialogueTweak.CrossModHelper;

/// <summary>
/// The replacement types for icon replacements. Don't change the name
/// </summary>
public enum IconType
{
    Happiness,
    Back,
    Shop,
    Extra
}

/// <summary>
/// DPR - Dialogue Panel Rework
/// </summary>
public static class DprHelper
{
    /// <summary>
    /// Replace the button icon with your custom icon.
    /// </summary>
    /// <param name="iconType">Which icon type you are replacing</param>
    /// <param name="npcTypes">Your NPC IDs is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. Use <b>-1</b> if you want to acess signs.</param>
    /// <param name="texturePath">You have to specify the texture that replaces icons. Use your texture's path. Use <b>Head</b> for button icons if you want to replace</param>
    /// <param name="availability">You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance of the button text from the icon</param>
    public static void ReplaceButtonIcon(IconType iconType, List<int> npcTypes, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        availability ??= () => true;
        dialogue.Call($"Replace{iconType.GetType().Name}ButtonIcon", npcTypes, texturePath, availability, frame, customTextOffset);
    }

    /// <summary>
    /// Replaces the button icon with your custom icon.
    /// </summary>
    /// <param name="iconType">Which icon type you are replacing</param>
    /// <param name="npcType">Your NPC ID is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. Use <b>-1</b> if you want to acess signs.</param>
    /// <param name="texturePath">You have to specify the texture that replaces icons. Use your texture's path. Use <b>Head</b> for button icons if you want to replace</param>
    /// <param name="availability">You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance of the button text from the icon</param>
    public static void ReplaceButtonIcon(IconType iconType, int npcType, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(iconType, new List<int>() {npcType}, texturePath, availability, frame, customTextOffset);

    /// <summary>
    /// Add a button for specific NPCs.
    /// </summary>
    /// <param name="npcTypes">NPC IDs is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. You can also tweak vanilla NPCs by using vanilla NPC ID.</param>
    /// <param name="buttonText">This is the text which will be shown in the button. It is <see cref="Func{TResult}"/> so you can use <see cref="Language.GetTextValue"/> or something else.</param>
    /// <param name="iconTexturePath">You have to specify the icon texture of the button. Use your texture's path. If you use "" or <see langword="null"/>, no icon will be shown.</param>
    /// <param name="hoverCallback">The action that will be called when the client hovers over the button. Use this to define the behavior when the button is pressed.</param>
    /// <param name="availability">You can decide if your button should be shown.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance of the button text from the icon</param>
    public static void AddButton(List<int> npcTypes, Func<string> buttonText, Func<string> iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        availability ??= () => true;
        dialogue.Call("AddButton", npcTypes, buttonText, iconTexturePath, hoverCallback, availability, frame, customTextOffset);
    }

    /// <summary>
    /// Add a button for specific NPCs.
    /// </summary>
    /// <param name="npcType">NPC ID is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. You can also tweak vanilla NPCs by using vanilla NPC ID.</param>
    /// <param name="buttonText">This is the text which will be shown in the button. It is <see cref="Func{TResult}"/> so you can use <see cref="Language.GetTextValue"/> or something else.</param>
    /// <param name="iconTexturePath">You have to specify the icon texture of the button. Use your texture's path. If you use "" or <see langword="null"/>, no icon will be shown.</param>
    /// <param name="hoverCallback">The action that will be called when the client hovers over the button. Use this to define the behavior when the button is pressed.</param>
    /// <param name="availability">You can decide if your button should be shown.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance of the button text from the icon</param>
    public static void AddButton(int npcType, Func<string> buttonText, Func<string> iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) =>
        AddButton(new List<int> {npcType}, buttonText, iconTexturePath, hoverCallback, availability, frame, customTextOffset);
}