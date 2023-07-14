<h1 align="center">更好的NPC对话面板</h1>

<div align="center">

[English](README.md) | 简体中文

一个完全重制了NPC对话框UI的Mod。

UI设计基于手机版UI但有所改善.

</div>

## 💻 编译

**注意**：你不应该使用tModLoader的编译，而应该使用代码IDE（如Visual Studio、Rider）的编译功能来编译此Mod，因为此Mod包含Nuget包

## 🤝 跨Mod支持

Mod制作者可以在 `PostSetupContent` 调用 `Mod.Call` 简单地为他们的NPC自定义按钮图标，也可以给NPC添加新的按钮。

如果你想给这个Mod添加支持，可以参考[这个文档](CrossModHelper/README.zh.md)