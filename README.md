# VanBurenTools
A launcher, settings editor, mod loader, and enhancement collection for Fallout Van Buren
- You can get the latest version of the launcher [here](https://github.com/kran27/VanBurenLauncher/raw/main/VBLauncher/bin/Release/VBLauncher.exe)
- You can get the game and other tools from [here](https://archive.org/details/f3demo)
### What This Project Consists Of:
<details> <summary>List of Features</summary>
  <br>
<details> <summary>Cut Content</summary>
  
- 2 Vehicles
- 3 Weapons
- 16 Creatures
- 15 Helmets
- 5 Item Icons
</details>
<details> <summary>Fixed Content</summary>
  
- 10 Maps
- Female Player Character
- In-Game Fonts
</details>
<details> <summary>General Tweaks</summary>
  
- Unlocked Camera Zoom
- Optional Alternate Camera Angles
- Easy Changes of .ini Settings
- Removed Useless Menu Buttons
</details>
<details> <summary>Graphics</summary>
  
- Support for DX11, and Vulkan
- MXAA & SSAA
- Alternate Texture Filtering
- Easy Switching of Resolution
- Mipmapping
- Phong Shading
</details>
</details>
<details> <summary>Projects</summary>
This repo consists of 4 projects
<details> <summary>AltUI</summary>
This is my UI library, used for the Launcher.
</details>
<details> <summary>VBLauncher</summary>
The Main UI, this handles everything done outside of the game, such as
- Editing Settings
- Loading Mods
- Creating your own mods
</details>
<details> <summary>VBEditor</summary>
This is a library that VBLauncher uses to work with the proprietary files used. this is done so that other projects can isolate this element and use it for their own purposes.
</details>
<details> <summary>VBExtender</summary>
A shim dll loaded by the launcher (not yet) that extends the capabilities of the game, and applys any fixes that must be done at runtime.
</details>
</details>

## How to use:
Put the launcher in the game directory, and open the options menu(s) and apply your desired tweaks before launching.
## Uses:
- [ImGui](https://github.com/kran27/imgui)
- [dgVoodoo2](http://dege.fw.hu)
- [DXVK](https://github.com/doitsujin/dxvk)
- [AltUI](https://github.com/kran27/AltUI)
- [DXSDK](https://www.microsoft.com/en-ca/download/details.aspx?id=6812)
