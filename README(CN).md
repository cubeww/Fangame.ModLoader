# Fangame.ModLoader

Fangame.ModLoader是一个C#编写的工具，可以让GameMaker编写的I wanna同人游戏支持“加载Mod”功能，例如PlayOnline、DbgHelper、Skinning...

![image-20250602182834771](D:\Projects\CSharp\Fangame.ModLoader\Doc\image-20250602182834771.png)

## 为什么？

传统的Mod方法一般需要手动反编译游戏文件，然后使用GameMaker等软件修改游戏文件，最后打包成新的exe。这种方法不仅费时费力，而且每次游戏更新都要重新修改一遍。

本工具为以上流程提供了一种通用的方法，自动修改游戏文件并打包成exe，并且可以根据需求启用/关闭某些Mod。

另外，本工具在每次运行时都将Mod后的游戏放入一个单独的文件夹，避免了对原始游戏文件夹的污染。同时也没有必要分发庞大的Mod后的游戏文件，只需分发Mod本身就可以了。

## 支持的游戏

Mod加载器使用了GM8游戏文件解析库以及UndertaleModLib解析库，支持大部分GameMaker8、GameMaker Studio（非YYC）的游戏。对于其它游戏，Mod加载器将不会解析游戏数据，但是支持编写自定义Mod逻辑，对最终**二进制文件**进行额外处理。

## 使用方法

从**Releases**页面下载Mod加载器程序并解压缩，你将看到如下结构：

```
Fangame.ModLoader/
	Mods/
	Running/
	Fangame.ModLoader.Gui.exe
	...
```

其中**Fangame.ModLoader.Gui.exe**表示**加载器本体**，**Mods**文件夹存放所有**可用的Mod**，**Running**文件夹存放**Mod之后的游戏**（用户不需要关注，因为加载器会自动运行它们，而且会在每次加载器启动时进行清理！）

要想Mod游戏，请运行**Fangame.ModLoader.Gui.exe**，在左边的列表框勾选要启用的Mod名称，最后将游戏可执行文件（.exe）拖到输出文本框上即可，Mod加载器会自动开始加载Mod并启动游戏。

## Mod开发

开发Mod需要一些GameMaker以及C#的编程知识。请参考 [Develop](./Develop(CN).md)

## 致谢

- [OpenGMK](https://github.com/OpenGMK/OpenGMK) GM8游戏数据的解析方法
- [UndertaleModTool](https://github.com/UnderminersTeam/UndertaleModTool) GMS游戏数据的解析库