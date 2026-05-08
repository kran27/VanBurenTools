@echo off
rem ============================================================================
rem  VBExtender build script (32-bit binkw32.dll proxy + Detours hooks).
rem
rem  Run from the "x86 Native Tools Command Prompt for VS 2022" so cl/link
rem  pick up the right INCLUDE/LIB for x86. The script auto-detects an active
rem  cl.exe and bails with a hint if you're in a plain cmd.exe instead.
rem
rem  Output: binkw32.dll in this folder. Copy it into your F3 install,
rem  overwriting the existing binkw32.dll. Object files go into .\build\.
rem ============================================================================

setlocal

rem --- legacy DirectX SDK (June 2010) for D3DX11tex.h + D3DX11.lib -------------
rem  imguiextensions.h includes <D3DX11tex.h> and pragma-links D3DX11.lib;
rem  these were last shipped in the June 2010 DirectX SDK and don't exist in
rem  the modern Windows SDK. Prepend that SDK to INCLUDE/LIB so cl/link find
rem  them. Override DXSDK_DIR if the SDK lives somewhere else.
if "%DXSDK_DIR%"=="" set "DXSDK_DIR=C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)"
if exist "%DXSDK_DIR%\Include\D3DX11tex.h" (
    set "INCLUDE=%DXSDK_DIR%\Include;%INCLUDE%"
    set "LIB=%DXSDK_DIR%\Lib\x86;%LIB%"
) else (
    echo [ERROR] D3DX11tex.h not found under "%DXSDK_DIR%\Include".
    echo Set DXSDK_DIR to your June 2010 DirectX SDK install path and retry.
    exit /b 1
)

rem --- sanity: are we in a developer prompt? -----------------------------------
where cl.exe >nul 2>nul
if errorlevel 1 (
    echo [ERROR] cl.exe not on PATH.
    echo Run this from the "x86 Native Tools Command Prompt for VS 2022".
    echo A 64-bit prompt will fail with /MACHINE:X86 mismatches.
    exit /b 1
)

rem Make sure the active cl is targeting x86. cl prints its target arch in
rem its banner; we detect the GOOD case ("for x86") and bail otherwise.
rem (Avoid space-separated findstr patterns — those are OR'd by token, so
rem "for x64 for ARM" would also match "for x86" via the "for" token.)
cl 2>&1 | findstr /c:"for x86" >nul
if errorlevel 1 (
    echo [ERROR] cl.exe is not targeting x86. VBExtender must build x86
    echo because F3.exe is 32-bit. Open the "x86 Native Tools Command
    echo Prompt for VS 2022" and try again.
    exit /b 1
)

rem --- output dirs --------------------------------------------------------------
set HERE=%~dp0
set HERE=%HERE:~0,-1%
set OUT_DIR=%HERE%\build
if not exist "%OUT_DIR%" mkdir "%OUT_DIR%"

rem --- compiler flags -----------------------------------------------------------
set CFLAGS=/nologo /LD /MD /O2 /EHsc /std:c^+^+17 ^
    /I"%HERE%" ^
    /I"%HERE%\ImGui" ^
    /I"%HERE%\ImGui\backends" ^
    /I"%HERE%\Detours\src" ^
    /I"%HERE%\DiscordRPC\include" ^
    /I"%HERE%\rapidjson\include" ^
    /I"%HERE%\MemoryModule" ^
    /DWIN32 /D_WINDOWS /D_USRDLL /DNDEBUG ^
    /DUNICODE /D_UNICODE ^
    /DNOMINMAX /DWIN32_LEAN_AND_MEAN ^
    /Fo"%OUT_DIR%\\" ^
    /Fd"%OUT_DIR%\\"

rem --- sources ------------------------------------------------------------------
set SOURCES=^
    "%HERE%\main.cpp" ^
    "%HERE%\ImGui\imgui.cpp" ^
    "%HERE%\ImGui\imgui_demo.cpp" ^
    "%HERE%\ImGui\imgui_draw.cpp" ^
    "%HERE%\ImGui\imgui_tables.cpp" ^
    "%HERE%\ImGui\imgui_widgets.cpp" ^
    "%HERE%\ImGui\backends\imgui_impl_dx11.cpp" ^
    "%HERE%\ImGui\backends\imgui_impl_win32.cpp" ^
    "%HERE%\Detours\src\detours.cpp" ^
    "%HERE%\Detours\src\creatwth.cpp" ^
    "%HERE%\Detours\src\disasm.cpp" ^
    "%HERE%\Detours\src\image.cpp" ^
    "%HERE%\Detours\src\modules.cpp" ^
    "%HERE%\DiscordRPC\src\discord_rpc.cpp" ^
    "%HERE%\DiscordRPC\src\rpc_connection.cpp" ^
    "%HERE%\DiscordRPC\src\serialization.cpp" ^
    "%HERE%\DiscordRPC\src\connection_win.cpp" ^
    "%HERE%\DiscordRPC\src\discord_register_win.cpp" ^
    "%HERE%\MemoryModule\MemoryModule.c"

rem --- linker flags + libs ------------------------------------------------------
rem  /DEF makes binkw32.dll re-export Bink's API to F3.exe — without this F3
rem  rejects the proxy with "entry point not found". /MACHINE:X86 forces x86
rem  to match F3.exe (32-bit) regardless of the prompt.
rem No /DEF — the original .vcxproj has an empty <ModuleDefinitionFile/>,
rem so exports come solely from proxy.h's __declspec(dllexport) decls. The
rem checked-in binkw32.def references f_* symbols that don't exist anywhere
rem in the source (it's left over from a different proxy scheme). Adding it
rem here would force the link to fail with 85 unresolved externals.
set LDFLAGS=/link /DLL /MACHINE:X86 ^
    /OUT:"%HERE%\binkw32.dll" ^
    /PDB:"%OUT_DIR%\binkw32.pdb" ^
    user32.lib gdi32.lib ^
    d3d11.lib dxgi.lib d3dcompiler.lib ^
    psapi.lib ole32.lib advapi32.lib shell32.lib

rem --- build --------------------------------------------------------------------
echo [build] cl %SOURCES% ...
cl %CFLAGS% %SOURCES% %LDFLAGS%
if errorlevel 1 (
    echo [ERROR] build failed
    exit /b 1
)

echo.
echo [build] OK — %HERE%\binkw32.dll
echo Copy it into your F3 install (overwrite the existing binkw32.dll).
endlocal
