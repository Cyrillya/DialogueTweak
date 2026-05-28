using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

// 记得把命名空间改成你自己的！
namespace DialogueTweak.CrossModHelper;

/// <summary>
/// 更好的NPC对话框(DialogueTweak)的跨Mod支持辅助类
/// </summary>
public static class DialogueTweakHelperZh
{
    /// <summary>
    /// 将按钮图标替换为你自定义的图标。
    /// </summary>
    /// <param name="interactionTypeName">你想要替换图标的交互类型。使用NPCInteraction的类名，例如 "OpenShop"、"GuideTip"、"NurseHeal"、"AnglerQuest"。</param>
    /// <param name="npcType">你需要表明你想要覆盖的NPC对象的ID，使用<see cref="ModContent.NPCType"/>来获取你的Mod中相应NPC的ID. 如果要覆盖标牌请使用<b>-1</b></param>
    /// <param name="texturePath">你需要表明用于替代图标的贴图. 请输入贴图路径. 如果你想要使用NPC的头像贴图请直接输入<b>Head</b>. 如果要使用当前任务鱼作为图标请输入 <b>QuestFish</b></param>
    /// <param name="availability">你可以决定是否应用该替代. 这对于有多种功能并想要不同的图标贴图的NPC来说十分有用</param>
    /// <param name="frame">你可以自定义覆盖贴图的绘制帧. 以在不同状况下显示一个贴图的不同部分</param>
    /// <param name="customTextOffset">您可以自定义包含文本的边界框的左侧到按钮框的左侧的距离。边界框(boundingBox)是整个按钮中除图标外的右侧部分，文本将绘制在这个隐形的框的中心。参考"how_offset_works.png"中的图文解释</param>
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
    /// 在特定条件下禁用NPC的对话框重制
    /// </summary>
    /// <param name="npcType">你需要表明NPC对象的ID，使用<see cref="ModContent.NPCType"/>来获取你的Mod中相应NPC的ID. 你也可以用原版NPC的ID来修改原版NPC</param>
    /// <param name="disableCondition">在何种情况下对话框重制会被禁用。留空会使其在任何情况下均被禁用</param>
    public static void DisablePanelRework(int npcType, Func<bool> disableCondition = null) {
        if (!ModLoader.TryGetMod("DialogueTweak", out var dialogue)) {
            return;
        }

        disableCondition ??= () => true;
        dialogue.Call("DisablePanelRework", npcType, disableCondition);
    }
}
