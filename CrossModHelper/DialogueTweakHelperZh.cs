using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

// 记得把命名空间改成你自己的！
namespace DialogueTweak.CrossModHelper;

/// <summary>
/// 更好的NPC对话框(DialogueTweak)的跨Mod支持辅助类
/// </summary>
public static class DialogueTweakHelperZh
{
    /// <summary>
    /// 替换图标的可用替换类型
    /// 不要改名字
    /// </summary>
    public enum ReplacementType
    {
        Happiness,
        Back,
        Shop,
        Extra
    }

    /// <summary>
    /// 将按钮图标替换为你自定义的图标。
    /// </summary>
    /// <param name="replacementType">你想要替代的图标类型</param>
    /// <param name="npcType">你需要表明你想要覆盖的NPC对象的ID，使用<see cref="ModContent.NPCType"/>来获取你的Mod中相应NPC的ID. 如果要覆盖标牌请使用<b>-1</b></param>
    /// <param name="texturePath">你需要表明用于替代图标的贴图. 请输入贴图路径. 如果你想要使用NPC的头像贴图替换Shop和Extra按钮图标请直接输入<b>Head</b></param>
    /// <param name="availability">你可以决定是否应用该替代. 这对于有多种功能并想要不同的图标贴图的NPC来说十分有用</param>
    /// <param name="frame">你可以自定义覆盖贴图的绘制帧. 以在不同状况下显示一个贴图的不同部分</param>
    /// <param name="customTextOffset">您可以自定义包含文本的边界框的左侧到按钮框的左侧的距离。边界框(boundingBox)是整个按钮中除图标外的右侧部分，文本将绘制在这个隐形的框的中心。参考“how_offset_works.png”中的图文解释</param>
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
    /// 为指定的NPC添加按钮
    /// </summary>
    /// <param name="npcType">你需要表明你想要覆盖的NPC对象的ID，使用<see cref="ModContent.NPCType"/>来获取你的Mod中相应NPC的ID. 你也可以用原版NPC的ID来修改原版NPC</param>
    /// <param name="buttonText">这是按钮将会显示的文本. 类型为<see cref="Func{TResult}"/>因此你可以使用<see cref="Language.GetTextValue"/>来获取翻译后的文本。也可以在不同情况下显示不同的文本</param>
    /// <param name="iconTexturePath">你需要表明用于图标的贴图. 请输入贴图路径. 不填或填 <see langword="null"/>的话就不会显示图标</param>
    /// <param name="hoverCallback">当客户端将鼠标悬停在按钮上时将调用的操作. 使用它来定义按下按钮时的行为</param>
    /// <param name="availability">你可以决定是否显示该按钮</param>
    /// <param name="frame">你可以自定义按钮图标的绘制帧. 以在不同状况下显示一个贴图的不同部分</param>
    /// <param name="customTextOffset">您可以自定义包含文本的边界框的左侧到按钮框的左侧的距离。边界框(boundingBox)是整个按钮中除图标外的右侧部分，文本将绘制在这个隐形的框的中心。参考“how_offset_works.png”中的图文解释</param>
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