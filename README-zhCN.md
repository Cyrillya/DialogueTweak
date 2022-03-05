# 更好的NPC对话面板
这是一个完全重制了NPC对话框UI的Mod，UI设计基于手机版UI但有所改善.

# 通过Mod.Call自定义图标
以下是所有可选的覆盖类型.
### ReplaceExtraButtonIcon
```"ReplaceExtraButtonIcon", int NPCID, string texture, [Func<bool> replacementAvailable]```更改指定NPC的Extra图标，阅读下文了解有关参数的详细信息.

### ReplaceShopButtonIcon
```"ReplaceShopButtonIcon", int NPCID, string texture, [Func<bool> replacementAvailable]```更改指定NPC的Shop图标，阅读下文了解有关参数的详细信息.

## 参数
### 1.) 覆盖类型 - ```string```
第一个参数应为你想要的覆盖类型. 覆盖Extra图标请使用**ReplaceExtraButtonIcon**，覆盖Shop图标请使用**ReplaceShopButtonIcon**.

### 2.) NPC ID - ```int```
你需要表明你想要覆盖的NPC对象的ID，使用```ModContent.NPCType<>()```来获取你的Mod中相应NPC的ID.

### 3.) 贴图 - ```string```
你需要表明用于替代图标的贴图. 请输入贴图路径. 如果你想要使用NPC的头像贴图请直接输入**Head**

### 4.) 可见性 - ```Func<bool>```
你可以决定是否使用该覆盖贴图. 这对于有多种功能并想要不同的图标贴图的NPC来说十分有用.

## 使用例
以下是一个为自己的NPC自定义Shop图标的完整的例子:
```CSharp
public override void PostSetupContent() {
    Mod dialogueTweak = ModLoader.GetMod("DialogueTweak");
    if (dialogueTweak != null) {
        dialogueTweakMod.Call(
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
    Mod dialogueTweak = ModLoader.GetMod("DialogueTweak");
    if (dialogueTweak != null) {
        dialogueTweakMod.Call(
            "ReplaceShopButtonIcon",
            NPCID.Guide, // NPC ID，这里是向导
            "Head"); // 这样的话就会显示NPC的头像贴图
    }
}
```
