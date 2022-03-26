# 更好的NPC对话面板
这是一个完全重制了NPC对话框UI的Mod，UI设计基于手机版UI但有所改善.

# Mod.Call (替代项)
以下是所有可选的覆盖类型.
### ReplaceExtraButtonIcon
```"ReplaceExtraButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```更改指定NPC的Extra图标，阅读下文了解有关参数的详细信息.

### ReplaceShopButtonIcon
```"ReplaceShopButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```更改指定NPC的Shop图标，阅读下文了解有关参数的详细信息.

### ReplaceHappinessButtonIcon
```"ReplaceHappinessButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```更改指定NPC的快乐值按钮图标，阅读下文了解有关参数的详细信息.

### ReplaceBackButtonIcon
```"ReplaceBackButtonIcon", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```更改指定NPC的退出按钮图标，阅读下文了解有关参数的详细信息.

### ReplacePortrait
```"ReplacePortrait", int/List<int> NPCIDs, string texture, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```更改指定NPC的左上角肖像，阅读下文了解有关参数的详细信息.

## 参数
### 1.) 覆盖类型 - ```string```
第一个参数应为你想要的覆盖类型. 类型表已在上方列出.

### 2.) NPC ID - ```int/List<int>```
你需要表明你想要覆盖的NPC对象的ID，使用```ModContent.NPCType<>()```来获取你的Mod中相应NPC的ID. 如果要覆盖标牌请使用**-1**

### 3.) 贴图 - ```string```
你需要表明用于替代图标/肖像的贴图. 请输入贴图路径. 如果你想要使用NPC的头像贴图替换Shop和Extra按钮图标请直接输入**Head**. 对于覆盖肖像，如果你想要禁止原肖像绘制请输入**None**.

### 4.) 可见性 - ```Func<bool>```
你可以决定是否使用该覆盖贴图. 这对于有多种功能并想要不同的图标贴图的NPC来说十分有用.

### 5.) 帧 - ```Func<Rectangle>```
你可以自定义覆盖贴图的绘制帧. 以在不同状况下显示一个贴图的不同部分.

## 使用例
以下是一个为自己的NPC自定义Shop图标的完整的例子:
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweak.Call(
            "ReplaceShopButtonIcon",
            ModContent.NPCType<NPCs.MyNPC>(), // NPC ID
            "TheMod/Assets/NPCShopIcon", // 贴图路径
            (Func<bool>)(() => Main.LocalPlayer.direction >= 0)); // 只有当玩家朝右时才会显示
    }
}
```
以下代码可让向导的“帮助”按钮贴图显示为向导的头像
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
        dialogueTweak.Call(
            "ReplaceShopButtonIcon",
            NPCID.Guide, // NPC ID，这里是向导
            "Head"); // 这样的话就会显示NPC的头像贴图
    }
}
```

# Mod.Call (绘制项)
### OnPostPortraitDraw
```"OnPostPortraitDraw", Action<SpriteBatch, Color, Rectangle> drawingAction```在肖像绘制后绘制你想要的东西.

### OnPreNPCPortraitDraw
```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```在NPC肖像绘制前绘制你想要的东西.

### OnPostNPCPortraitDraw
```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, NPC> drawingAction```在NPC肖像绘制后绘制你想要的东西.

### OnPreSignPortraitDraw
```"OnPreNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```在标牌肖像绘制前绘制你想要的东西.

### OnPostSignPortraitDraw
```"OnPostNPCPortraitDraw", Action<SpriteBatch, Color, Rectangle, int> drawingAction```在标牌肖像绘制后绘制你想要的东西.

## 参数
### 1.) 绘制钩子类型 - ```string```
输入你想要的绘制钩子的名称，以上是一个钩子类型的列表

### 2.) 绘制Action - ```Action<SpriteBatch, Color, Rectangle, int/NPC>```
你要传入你的绘制方法

 **```SpriteBatch``` - 用于绘制的SpriteBatch实例**
 
 **```Color``` - 该NPC/标牌在绘制名称时应使用的颜色**
 
 **```Rectangle``` - 对话面板的Rectangle实例**
 
 **```int``` - 标牌的索引， ```NPC``` - NPC实例**

## 使用例
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

# Mod.Call (添加按钮)
目前只有一个添加按钮的Mod.Call，不过我觉得应该够用了.
### AddButton
```"AddButton", int/List<int> NPCIDs, Func<string> buttonText, string texture, Action hoverAction, [Func<bool> replacementAvailable], [Func<Rectangle> frame]```添加一个新按钮.

## Arguments
### 1.) 按钮类型 - ```string```
填“AddButton”就完事了.

### 2.) NPC ID - ```int/List<int>```
你需要表明你想要覆盖的NPC对象的ID，使用```ModContent.NPCType<>()```来获取你的Mod中相应NPC的ID. 你也可以用原版NPC的ID来修改原版NPC.

### 3.) 按钮文本 - ```Func<string>```
这是按钮将会显示的文本. 类型为```Func<string>```因此你可以使用Language.GetTextValue或者其他别的玩意.

### 4.) 图标贴图 - ```string```
你需要表明用于图标的贴图. 请输入贴图路径. 不填的话就不会显示图标.

### 5.) 悬停Action - ```Action```
当客户端将鼠标悬停在按钮上时将调用的操作. 使用它来定义按下按钮时的行为.

### 6.) 可见性 - ```Func<bool>```
你可以决定是否显示该按钮.

### 7.) 帧 - ```Func<Rectangle>```
你可以自定义图标贴图的绘制帧. 以在不同状况下显示一个贴图的不同部分.

## Examples
以下示例使用此Mod的关闭按钮贴图(即Button_Back)添加了一个按钮，显示为“关闭”. 当玩家单击它时将显示一条消息。
```CSharp
public override void PostSetupContent() {
	if (ModLoader.TryGetMod("DialogueTweak", out Mod dialogueTweak)) {
		dialogueTweakLoaded = true;
		dialogueTweak.Call(
			"AddButton",
			NPCID.Angler,
			(Func<string>)(() => Language.GetTextValue("LegacyInterface.52")), // "关闭"
			"DialogueTweak/Interfaces/Assets/Button_Back", // 直接引用该Mod的贴图.
			(Action)(() => {
				if (Main.mouseLeft)
                    Main.NewText("这是假的关闭按钮哦");
			}));
	}
}
```