#include "includes.h"
#include "proxy.h"

bool init = false;
FILE* fp;
bool close = false;

void LoadASIModules(const std::wstring& directory)
{
	std::vector<std::wstring> asifiles;
	WIN32_FIND_DATA findFileData;
	HANDLE hFind;
	std::wstring searchPath = directory + L"\\*.asi";
	hFind = FindFirstFile(searchPath.c_str(), &findFileData);
	if (hFind != INVALID_HANDLE_VALUE)
	{
		do
		{
			std::wstring asiPath = directory + L"\\" + findFileData.cFileName;
			asifiles.push_back(asiPath);
		} while (FindNextFile(hFind, &findFileData) != 0);
		FindClose(hFind);
	}

	for (const auto& asiPath : asifiles)
	{
		HMODULE module = LoadLibrary(asiPath.c_str());
		if (module)
		{
			std::wcout << "Loaded .asi module: " << asiPath << std::endl;
		}
		else
		{
			std::wcerr << "Failed to load .asi module: " << asiPath << std::endl;
		}
	}
}

void RPCInit()
{
	DiscordEventHandlers Handler;
	memset(&Handler, 0, sizeof(Handler));
	Discord_Initialize("1115529208584732714", &Handler, 1, NULL);
}

void WINAPI RPCUpdate()
{
	DiscordRichPresence discordPresence;
	memset(&discordPresence, 0, sizeof(discordPresence));
	discordPresence.startTimestamp = time(nullptr);
	discordPresence.largeImageKey = "f3logo";
	while (true)
	{
		auto map = mapName();
		if (map[0] != '\0') {
			auto plyr = getPlayerptr();
			if (*((_DWORD*)plyr + 0x79))
			{
				discordPresence.details = map;
				const char* fileName = ReadString(plyr + 0x190);
				std::string charName(fileName, strlen(fileName) - 4);

				auto ap = Read<int>(plyr + 0x174);
				auto ent = (int)RTDynamicCast((uint32*)plyr, NULL, Entity, GameEntity, NULL);
				auto mhp = Read<int>(ent + 0x20);
				auto hp = Read<int>(ent + 0x24);

				discordPresence.state = formatString("%s | HP: %d/%d | AP: %d", charName.c_str(), hp, mhp, ap);

				goto notmm;
			}
		}
		discordPresence.details = "At Main Menu";
		discordPresence.state = "";

	notmm:

		Discord_UpdatePresence(&discordPresence);
		Sleep(250);
		if (close) break;
	}
}

int WINAPI main()
{
	GetAddresses();
	// Allocate and hide a console
	//AllocConsole();
	//ToggleConsole();
	//SetConsoleTitleA("F3 Debug Log");
	//// Redirect output to the console
	//freopen_s(&fp, "CONOUT$", "w", stdout);
	//freopen_s(&fp, "CONOUT$", "w", stderr);

	InitHooks();

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
			RegisterConsoleCommand("EntityInfo", EntityInfo);

			WritePtr<int>(F3 + 0x307EF8, { 0x4 }, 0);

			DebugAndConsole("Van Buren Extender %s Loaded and Initialized", version);

			DebugAndConsole("Original Bink Base Address: %p", binkHMM);

			//TODO: Add config for borderless fullscreen somehow
			//auto hwnd = GetForegroundWindow();
			//int w = GetSystemMetrics(SM_CXSCREEN);
			//int h = GetSystemMetrics(SM_CYSCREEN);
			//SetWindowLongPtr(hwnd, GWL_STYLE, WS_VISIBLE | WS_POPUP);
			//SetWindowPos(hwnd, HWND_TOP, 0, 0, w, h, SWP_FRAMECHANGED);

			WCHAR dllPath[MAX_PATH];
			GetModuleFileName(dll_handle, dllPath, MAX_PATH);
			std::wstring directory = std::filesystem::path(dllPath).parent_path().wstring() + L"\\Plugins";
			LoadASIModules(directory);

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
				Level = Read<int>(gent + 0xBC);
				Locked = Read<char>(gent + 0xBD);
				Race = Read<int>(gent + 0xC8);
				Gender = Read<int>(gent + 0x34);
				Ethnicity = Read<int>(gent + 0xD4);
				SubRace = Read<int>(gent + 0xD8);
				Type = Read<int>(gent + 0xA0);
				LockDC = Read<int>(gent + 0xC0);
				// ent + 0x168 is inventory, gent[38] is attack mode
				Active = Read<char>(ent + 0x3A0);
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
	{
		dll_handle = hModule;
		CreateThread(nullptr, 0, reinterpret_cast<LPTHREAD_START_ROUTINE>(main), nullptr, 0, nullptr);
		RPCInit();
		CreateThread(nullptr, 0, reinterpret_cast<LPTHREAD_START_ROUTINE>(RPCUpdate), nullptr, 0, nullptr);
	}
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
		if (ImGui::GetCurrentContext())
		{
			ImGui_ImplDX11_Shutdown();
			ImGui_ImplWin32_Shutdown();
			ImGui::DestroyContext();
		}

		if (mainRenderTargetView) { mainRenderTargetView->Release(); mainRenderTargetView = nullptr; }
		if (p_context) { p_context->Release(); p_context = nullptr; }
		if (p_device) { p_device->Release(); p_device = nullptr; }
		SetWindowLongPtr(window, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(oWndProc));

		close = true;
		Sleep(255);

		FreeLibraryAndExitThread(dll_handle, 0);
		break;
	}
	return TRUE;
}