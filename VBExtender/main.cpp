#include "includes.h"

bool init = false;
FILE* fp;
bool close = false;

int WINAPI main()
{
	// Allocate and hide a console
	AllocConsole();
	ToggleConsole();
	SetConsoleTitleA("F3 Debug Log");
	// Redirect output to the console
	freopen_s(&fp, "CONOUT$", "w", stdout);
	freopen_s(&fp, "CONOUT$", "w", stderr);

	InitHooks();

	for (auto& str : entities)
	{
		str = "";
	}

	while (true)
	{
		if (!init && initReady && get_present_pointer()) {
			Sleep(50);

			const auto statAddr = GetAddr(SettingsBase, { 0x88C });
			const auto stats = Read<char>(statAddr);

			renderStats = stats;
			Write<char>(statAddr, 0);

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&reinterpret_cast<PVOID&>(p_present), detour_present);
			DetourTransactionCommit();

			RegisterConsoleCommand("TestCommand", TestFunction);
			RegisterConsoleCommand("GetVBEVersion", GetVBEVersion);
			RegisterConsoleCommand("ToggleConsole", ToggleConsole);
			RegisterConsoleCommand("SetHealth", HealthTest);
			RegisterConsoleCommand("EntityInfo", (PVOID)(F3 + 0x37030));

			WritePtr<int>(F3 + 0x307EF8, { 0x4 }, 0);

			DebugAndConsole("Van Buren Extender %s Loaded and Initialized", version);

			//TODO: Add config for borderless fullscreen somehow
			//auto hwnd = GetForegroundWindow();
			//int w = GetSystemMetrics(SM_CXSCREEN);
			//int h = GetSystemMetrics(SM_CYSCREEN);
			//SetWindowLongPtr(hwnd, GWL_STYLE, WS_VISIBLE | WS_POPUP);
			//SetWindowPos(hwnd, HWND_TOP, 0, 0, w, h, SWP_FRAMECHANGED);

			init = true;
		}

		Sleep(50);

		if (init)
		{
			if (GetAsyncKeyState(VK_END) & 1);

			auto ent = GetAddr(EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x0 });

			if (ent) {
				for (uint i = 0; i < 64; i++)
				{
					entities[i] = ReadString(EntityBase, { 0xE8, i * 4, 0x2C, static_cast<uint>(useNames ? 0x1C4 : 0x190) });
				}

				entPos[0] = Read<float>(ent + 0x4);
				entPos[1] = Read<float>(ent + 0x8);
				entPos[2] = Read<float>(ent + 0xC);
				AP = Read<int>(ent + 0x174);

				auto gent = (int)RTDynamicCast((uint32*)ent, 0, Entity, GameEntity, 0);

				MaxHP = Read<int>(gent + 0x20);
				HP = Read<int>(gent + 0x24);
				Special[0] = Read<int>(gent + 0xA0);
				Special[1] = Read<int>(gent + 0xA4);
				Special[2] = Read<int>(gent + 0xA8);
				Special[3] = Read<int>(gent + 0xAC);
				Special[4] = Read<int>(gent + 0xB0);
				Special[5] = Read<int>(gent + 0xB4);
				Special[6] = Read<int>(gent + 0xB8);
				Age = Read<int>(gent + 0xCC);
			}
			if (close) break;
		}
	}
}

BOOL APIENTRY DllMain(HMODULE hModule, const DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		dll_handle = hModule;
		CreateThread(nullptr, 0, reinterpret_cast<LPTHREAD_START_ROUTINE>(main), nullptr, 0, nullptr);
		break;
	case DLL_PROCESS_DETACH:
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(reinterpret_cast<PVOID*>(&OriginalOutputDebugStringA), HookedOutputDebugStringA);
		DetourDetach(reinterpret_cast<PVOID*>(&g_GetTickCountOriginal), GetTickCountHacked);
		DetourDetach(reinterpret_cast<PVOID*>(&g_TimeGetTimeOriginal), GetTickCountHacked);
		DetourDetach(reinterpret_cast<PVOID*>(&g_QueryPerformanceCounterOriginal), QueryPerformanceCounterHacked);
		DetourDetach(reinterpret_cast<PVOID*>(&p_present), detour_present);
		DetourTransactionCommit();

		fclose(fp);
		FreeConsole();
		ImGui_ImplDX11_Shutdown();
		ImGui_ImplWin32_Shutdown();
		ImGui::DestroyContext();

		if (mainRenderTargetView) { mainRenderTargetView->Release(); mainRenderTargetView = nullptr; }
		if (p_context) { p_context->Release(); p_context = nullptr; }
		if (p_device) { p_device->Release(); p_device = nullptr; }
		SetWindowLongPtr(window, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(oWndProc));

		close = true;
		Sleep(150);

		FreeLibraryAndExitThread(dll_handle, 0);
		break;
	}
	return TRUE;
}