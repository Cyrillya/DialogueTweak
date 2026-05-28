using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

// Change the namespace to yours!
namespace DialogueTweak.CrossModHelper;

/// <summary>
/// Cross-mod support helper class for Dialogue Panel Rework (DialogueTweak)
/// </summary>
public static class DialogueTweakHelper
{
    /// <summary>
    /// Replace the button icon with your custom icon.
    /// </summary>
    /// <param name="interactionTypeName">Which interaction's icon you are replacing. Use the class name of the NPCInteraction, e.g. "OpenShop", "GuideTip", "NurseHeal", "AnglerQuest".</param>
    /// <param name="npcType">Your NPC ID(s) is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. Use <b>-1</b> if you want to access signs.</param>
    /// <param name="texturePath">You have to specify the texture that replaces icons. Use your texture's path. Use <b>Head</b> if you want to display the NPC's head. Use <b>QuestFish</b> if you want to display available quest fish.</param>
    /// <param name="availability">You can decide if your replacement is used. This is useful if your NPC has multiple functions that display different icons.</param>
    /// <param name="frame">You can customize the frame of the texture. It is useful to display different parts of the texture in different situations.</param>
    /// <param name="customTextOffset">You can customize the distance from the left side of the box containing the text to the left side of the button. The invisible box is the right part of the entire button excluding the icon, and the text will be drawn in its center. Check "how_offset_works.png" for image explanation</param>
    public static void ReplaceButtonIcon(string interactionTypeName, List<int> npcType, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        availability ??= () => true;
        dialogue.Call("ReplaceButtonIcon", interactionTypeName, npcType, texturePath, availability, frame, customTextOffset);
    }

    /// <inheritdoc cref="ReplaceButtonIcon(string, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(string interactionTypeName, int npcType, Func<string> texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(interactionTypeName, new List<int> {npcType}, texturePath, availability, frame, customTextOffset);

    /// <inheritdoc cref="ReplaceButtonIcon(string, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(string interactionTypeName, List<int> npcType, string texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(interactionTypeName, npcType, () => texturePath, availability, frame, customTextOffset);

    /// <inheritdoc cref="ReplaceButtonIcon(string, List{int}, Func{string}, Func{bool}, Func{Rectangle}, Func{float})"/>
    public static void ReplaceButtonIcon(string interactionTypeName, int npcType, string texturePath,
        Func<bool> availability = null, Func<Rectangle> frame = null, Func<float> customTextOffset = null) =>
        ReplaceButtonIcon(interactionTypeName, new List<int> {npcType}, () => texturePath, availability, frame, customTextOffset);

    /// <summary>
    /// Disable the panel rework for the given NPC type, if condition is met.
    /// </summary>
    /// <param name="npcType">NPC ID(s) is needed. Use <see cref="ModContent.NPCType"/> to submit your ID. You can also tweak vanilla NPCs by using vanilla NPC ID.</param>
    /// <param name="disableCondition">Under what condition should the rework panel be disabled. Leave it blank will cause it to be disabled at any time</param>
    public static void DisablePanelRework(int npcType, Func<bool> disableCondition = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        disableCondition ??= () => true;
        dialogue.Call("DisablePanelRework", npcType, disableCondition);
    }
}
