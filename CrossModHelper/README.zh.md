<h1 align="center">跨Mod支持</h1>

<div align="center">

[English](README.md) | 简体中文

</div>

## 辅助类

本Mod为Mod作者们提供了一个辅助类，以便他们添加跨Mod支持。直接将 [DialogueTweakHelperZh.cs](DialogueTweakHelperZh.cs) 文件复制粘贴到你的Mod代码中即可使用。

辅助类包含一些用于添加按钮和更改按钮图标的方法。这样你就不需要去用缺乏文档和参数提示的 `Mod.Call` 了。

辅助类本身带有文档，用起来方便快捷。

### 使用例

以下示例使用此Mod的关闭按钮贴图(即Button_Back)为渔夫添加了一个按钮，显示为“关闭”. 当玩家单击它时将显示一条消息。

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.AddButton(
	    NPCID.Angler,
	    () => Language.GetTextValue("LegacyInterface.52"), // "关闭"
	    "DialogueTweak/Interfaces/Assets/Button_Back", // 直接引用该Mod的贴图
	    () => {
		    if (Main.mouseLeft && Main.mouseLeftRelease)
			    Main.NewText("这是假的关闭按钮哦");
	    });
}
```

以下是一个为自己的NPC自定义Shop图标的完整的例子:

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.ReplaceButtonIcon(
	    DialogueTweakHelper.ReplacementType.Shop,
	    ModContent.NPCType<NPCs.MyNPC>(), // NPC ID
	    "TheMod/Assets/NPCShopIcon", // 贴图路径
	    () => Main.LocalPlayer.direction >= 0); // 只有当玩家朝右时才会显示
}
```

以下代码可让向导的“帮助”按钮贴图显示为向导的头像

```CSharp
public override void PostSetupContent() {
    DialogueTweakHelper.ReplaceButtonIcon(
	    DialogueTweakHelper.ReplacementType.Shop,
	    NPCID.Guide, // NPC ID，这里是向导
	    "Head"); // 这样的话就会显示NPC的头像贴图
}
```

## Mod.Call

`Mod.Call` 是各种Mod之间添加支持的主要方式。辅助类本质上也是调用 `Mod.Call` 来添加支持的

如果你想要了解 `Mod.Call`，可以看看tModLoader团队写的[这篇文档](https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate)。中文教程可以参考这篇[裙中世界网站的文档](https://fs49.org/2020/05/30/%e9%ad%94%e6%94%b9%e8%bf%9b%e9%98%b6-mod%e8%81%94%e5%8a%a8/)

你可以使用 `Mod.Call` 来为本Mod添加支持，但是更推荐使用本Mod提供的辅助类。

如果你想要访问对话框的绘制，你必须使用 `Mod.Call`。


### 使用 Mod.Call 访问绘制
#### OnPostPortraitDraw
```"OnPostPortraitDraw", Action<SpriteBatch, Color, Rectangle> drawingAction```在肖像绘制后绘制你想要的东西.

#### OnPreNPCPortraitDraw
```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```在NPC肖像绘制前绘制你想要的东西.

#### OnPostNPCPortraitDraw
```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```在NPC肖像绘制后绘制你想要的东西.

#### OnPreSignPortraitDraw
```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```在标牌肖像绘制前绘制你想要的东西.

#### OnPostSignPortraitDraw
```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```在标牌肖像绘制后绘制你想要的东西.

### 参数
#### 1.) 绘制钩子类型 - ```string```
输入你想要的绘制钩子的名称，以上是一个钩子类型的列表

#### 2.) 绘制Action - ```Action<SpriteBatch, Color, Rectangle, int/NPC>```
你要传入你的绘制方法

**```SpriteBatch``` - 用于绘制的SpriteBatch实例**

**```Color``` - 该NPC/标牌在绘制名称时应使用的颜色**

**```Rectangle``` - 对话面板的Rectangle实例**

**```int``` - 标牌的索引， ```NPC``` - NPC实例**

### 使用例
以下是一个在面板左上角绘制一个颜色为DiscoColor的某玩意的例子:
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