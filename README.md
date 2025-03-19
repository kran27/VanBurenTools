# VanBurenTools
[![](https://dyna-badge.vercel.app/api/discord?guild=470671750087180289)](https://discord.gg/tzF3YFu)<br>
Various tools and code relating to reverse engineering Fallout Van Buren
- You can get the latest version of the launcher [here](https://github.com/kran27/VanBurenLauncher/raw/main/VBLauncher/bin/Release/VBLauncher.exe)
- You can get Van Buren [here](https://vb.kran.gg/F3_Demo.rar)

## Repo Content:
### VBLauncher
The main thing. features include:
- Edit game and dgVoodoo settings
- Load mods with ModOrganizer 2 inspired UI (mostly functional)
- Edit game files [WIP]
- Extract game files from .grp (feature parity with ungrp, no other tools needed)
- Visual preview of game resources (models, textures, etc.) [WIP]
### VBExtender
A DLL loaded by the game. includes some patches and utilities, and an .ASI loader, not guaranteed to be stable.

## How to use:
Put the launcher in the game directory, and open the options menu(s) and apply your desired tweaks before launching.
## 3rd Party Libraries:
### VBLauncher:
- [AltUI](https://github.com/kran27/AltUI) for (most) UI elements
- [dgVoodoo2](http://dege.fw.hu) for in-game graphical improvements
- [DXVK](https://github.com/doitsujin/dxvk) for no good reason
- [Helix Toolkit](https://github.com/helix-toolkit/helix-toolkit) for 3D rendering/previews
- [HexBox](https://sourceforge.net/projects/hexbox/) for preview of some files
- [Pfim](https://github.com/nickbabcock/Pfim) for loading of .tga images
- [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) for editor UI
- [Veldrid](https://github.com/veldrid/veldrid) for ImGui.NET backend
- [ImGuiColorTextEditNet](https://github.com/kran27/ImGuiColorTextEditNet) for textual editors
### VBExtender:
- [Detours](https://github.com/microsoft/Detours) for hooking functions
- [discord-rpc](https://github.com/discord/discord-rpc) for Discord integration
- [ImGui](https://github.com/ocornut/imgui) for menus
- [MemoryModule](https://github.com/fancycode/MemoryModule) for containing & loading the dll it replaces
