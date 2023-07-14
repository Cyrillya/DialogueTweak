using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

// Change the namespace to yours!
namespace DialogueTweak.CrossModHelper;

/// <summary>
/// The replacement types for icon replacements. Don't change the name
/// </summary>
public enum ReplacementType
{
    Happiness,
    Back,
    Shop,
    Extra
}

/// <summary>
/// Cross-mod support helper class for Dialogue Panel Rework (Dialogue Tweak)
/// </summary>
public static class DialogueTweakHelper
{
    /// <summary>
    /// Replace the button icon with your custom icon.
    /// </summary>
    /// <param name="replacementType">Which icon type you are replacing</param>
    /// <param name="npcType">Your NPC ID(s) is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. Use <b>-1</b> if you want to acess signs.</param>
    /// <param name="texturePath">You have to specify the texture that replaces icons. Use your texture's path. Use <b>Head</b> for button icons if you want to replace</param>
    /// <param name="availability">You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance from the left side of the box containing the text to the left side of the button. The invisible box is the right part of the entire button excluding the icon, and the text will be drawn in its center. Check "how_offset_works.png" for image explanation</param>
    public static void ReplaceButtonIcon(ReplacementType replacementType, List<int> npcType, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        availability ??= () => true;
        dialogue.Call($"Replace{replacementType}ButtonIcon", npcType, texturePath, availability, frame, customTextOffset);
    }

    /// <inheritdoc cref="ReplaceButtonIcon(ReplacementType, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(ReplacementType replacementType, int npcType, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(replacementType, new List<int> {npcType}, texturePath, availability, frame, customTextOffset);
    
    /// <inheritdoc cref="ReplaceButtonIcon(ReplacementType, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(ReplacementType replacementType, List<int> npcType, string texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(replacementType, npcType, () => texturePath, availability, frame, customTextOffset);
    
    /// <inheritdoc cref="ReplaceButtonIcon(ReplacementType, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(ReplacementType replacementType, int npcType, string texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(replacementType, new List<int> {npcType}, () => texturePath, availability, frame, customTextOffset);

    /// <summary>
    /// Add a button for specific NPCs.
    /// </summary>
    /// <param name="npcType">NPC ID(s) is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. You can also tweak vanilla NPCs by using vanilla NPC ID.</param>
    /// <param name="buttonText">This is the text which will be shown in the button. It is <see cref="Func{TResult}"/> so you can use <see cref="Language.GetTextValue"/> or something else.</param>
    /// <param name="iconTexturePath">You have to specify the icon texture of the button. Use your texture's path. If you use "" or <see langword="null"/>, no icon will be shown.</param>
    /// <param name="hoverCallback">The action that will be called when the client hovers over the button. Use this to define the behavior when the button is pressed.</param>
    /// <param name="availability">You can decide if your button should be shown.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance from the left side of the box containing the text to the left side of the button. The invisible box is the right part of the entire button excluding the icon, and the text will be drawn in its center. Check "how_offset_works.png" for image explanation</param>
    public static void AddButton(List<int> npcType, Func<string> buttonText, Func<string> iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        availability ??= () => true;
        dialogue.Call("AddButton", npcType, buttonText, iconTexturePath, hoverCallback, availability, frame, customTextOffset);
    }

    /// <inheritdoc cref="AddButton(List{int}, Func{string}, Func{string}, Action, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void AddButton(int npcType, Func<string> buttonText, Func<string> iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) =>
        AddButton(new List<int> {npcType}, buttonText, iconTexturePath, hoverCallback, availability, frame, customTextOffset);

    /// <inheritdoc cref="AddButton(List{int}, Func{string}, Func{string}, Action, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void AddButton(List<int> npcType, Func<string> buttonText, string iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) =>
        AddButton(npcType, buttonText, () => iconTexturePath, hoverCallback, availability, frame, customTextOffset);

    /// <inheritdoc cref="AddButton(List{int}, Func{string}, Func{string}, Action, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void AddButton(int npcType, Func<string> buttonText, string iconTexturePath,
        Action hoverCallback, Func<bool> availability = null, Func<Rectangle> frame = null,
        Func<float> customTextOffset = null) =>
        AddButton(new List<int> {npcType}, buttonText, () => iconTexturePath, hoverCallback, availability, frame, customTextOffset);
}