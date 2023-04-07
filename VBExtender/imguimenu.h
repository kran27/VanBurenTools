#pragma once
#define WIN32_LEAN_AND_MEAN

#include "imguiextensions.h"

#define setuptext ImGui::SetCursorPosY(ImGui::GetCursorPosY() + 3); ImGui::SetCursorPosX(12); ImGui::Text(
#define setupcontrol ); ImGui::SameLine(); ImGui::SetCursorPosY(ImGui::GetCursorPosY() - 3); ImGui::SetCursorPosX(100); ImGui::SetNextItemWidth(ImGui::GetContentRegionAvail().x);

#pragma region init/setup
struct ImGui_ImplDX11_Data;
HINSTANCE dll_handle;

typedef long(__stdcall* present)(IDXGISwapChain*, UINT, UINT);
present p_present;

typedef void(__stdcall* D3D11DrawIndexedInstancedIndirectHook) (ID3D11DeviceContext* pContext, ID3D11Buffer* pBufferForArgs, UINT AlignedByteOffsetForArgs);
D3D11DrawIndexedInstancedIndirectHook phookD3D11DrawIndexedInstancedIndirect = NULL;

typedef void(__stdcall* D3D11DrawIndexedInstancedHook) (ID3D11DeviceContext* pContext, UINT IndexCountPerInstance, UINT InstanceCount, UINT StartIndexLocation, INT BaseVertexLocation, UINT StartInstanceLocation);
D3D11DrawIndexedInstancedHook phookD3D11DrawIndexedInstanced = NULL;

typedef void(__stdcall* D3D11DrawHook) (ID3D11DeviceContext* pContext, UINT VertexCount, UINT StartVertexLocation);
D3D11DrawHook phookD3D11Draw = NULL;

typedef void(__stdcall* D3D11DrawIndexedHook) (ID3D11DeviceContext* pContext, UINT IndexCount, UINT StartIndexLocation, INT BaseVertexLocation);
D3D11DrawIndexedHook phookD3D11DrawIndexed = NULL;

bool get_present_pointer()
{
	DXGI_SWAP_CHAIN_DESC sd;
	ZeroMemory(&sd, sizeof(sd));
	sd.BufferCount = 2;
	sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	sd.OutputWindow = GetForegroundWindow();
	sd.SampleDesc.Count = 1;
	sd.Windowed = TRUE;
	sd.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;

	IDXGISwapChain* swap_chain;
	ID3D11Device* device;

	const D3D_FEATURE_LEVEL feature_levels[] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_0, };
	if (D3D11CreateDeviceAndSwapChain(
		NULL,
		D3D_DRIVER_TYPE_HARDWARE,
		NULL,
		0,
		feature_levels,
		2,
		D3D11_SDK_VERSION,
		&sd,
		&swap_chain,
		&device,
		nullptr,
		nullptr) == S_OK)
	{
		void** p_vtable = *reinterpret_cast<void***>(swap_chain);
		swap_chain->Release();
		device->Release();
		//context->Release();
		p_present = (present)p_vtable[8];
		phookD3D11DrawIndexedInstancedIndirect = (D3D11DrawIndexedInstancedIndirectHook)p_vtable[39];
		phookD3D11DrawIndexedInstanced = (D3D11DrawIndexedInstancedHook)p_vtable[20];
		phookD3D11DrawIndexed = (D3D11DrawIndexedHook)p_vtable[12];
		phookD3D11Draw = (D3D11DrawHook)p_vtable[13];
		return true;
	}
	return false;
}

WNDPROC oWndProc;
extern LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

LRESULT __stdcall WndProc(const HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
		return true;

	return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
}

#pragma endregion

bool show = false;
bool lastShow;
bool imguiinit = false;
HWND window = NULL;
uint tab = 1;
ID3D11DeviceContext* p_context = NULL;
ID3D11RenderTargetView* mainRenderTargetView = NULL;
ID3D11ShaderResourceView* attackCursor = nullptr;
ID3D11ShaderResourceView* defaultCursor = nullptr;
ID3D11ShaderResourceView* guiCursor = nullptr;
ID3D11ShaderResourceView* pickupCursor = nullptr;
ID3D11ShaderResourceView* targetCursor = nullptr;
ID3D11ShaderResourceView* textCursor = nullptr;

bool usePickupCursor = false;
bool showMouseOver = false;
float speed = 1.0f;
bool renderStats = false;
bool altStats = false;

ImFont* hrFont;

inline void DrawGOBJData(uint32* ent)
{
	auto gobj = (int)RTDynamicCast(ent, 0, Entity, GameUsableObject, 0);
}

inline void DrawGITMData(uint32* ent)
{
	auto gitm = (int)RTDynamicCast((uint32*)ent, 0, Entity, GameItem, 0);
}

#pragma region Stuff before render code

class Model {
private:
	UINT ModelStride;
	UINT ModelvscWidth;
	UINT ModelpscWidth;
	UINT ModelpsDescrFORMAT;
public:
	Model(UINT Stride, UINT vscWidth, UINT pscWidth, UINT psDescrFORMAT) { ModelStride = Stride; ModelvscWidth = vscWidth; ModelpscWidth = pscWidth; ModelpsDescrFORMAT = psDescrFORMAT; };

	bool IsModel(UINT Stride, UINT vscWidth, UINT pscWidth) { if (Stride == ModelStride && vscWidth == ModelvscWidth && pscWidth == ModelpscWidth)return true; else return false; };
	bool IsModel(UINT Stride, UINT vscWidth, UINT pscWidth, UINT psDescrFORMAT) { if (Stride == ModelStride && vscWidth == ModelvscWidth && pscWidth == ModelpscWidth && psDescrFORMAT == ModelpsDescrFORMAT)return true; else return false; };
};

int modelstride = NULL;
int modelvscwidth = NULL;
int modelpscwidth = NULL;
int modelpsdescrformat = NULL;
bool cycle = false;

inline Model TestModel() {
	Model x(modelstride, modelvscwidth, modelpscwidth, modelpsdescrformat);
	return x;
}

void cycle_values()
{
	modelpscwidth++;
	if (modelpscwidth > 100)
	{
		modelpscwidth = 0;
		modelvscwidth++;
		if (modelvscwidth > 100)
		{
			modelvscwidth = 0;
		}
	}
}

static long __stdcall detour_present(IDXGISwapChain* p_swap_chain, UINT sync_interval, UINT flags)
{
	if (!imguiinit)
	{
		if (SUCCEEDED(p_swap_chain->GetDevice(__uuidof(ID3D11Device), (void**)&p_device)))
		{
			p_device->GetImmediateContext(&p_context);
			DXGI_SWAP_CHAIN_DESC sd;
			p_swap_chain->GetDesc(&sd);
			window = sd.OutputWindow;
			ID3D11Texture2D* pBackBuffer;
			p_swap_chain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackBuffer);
			p_device->CreateRenderTargetView(pBackBuffer, NULL, &mainRenderTargetView);
			pBackBuffer->Release();
			oWndProc = (WNDPROC)SetWindowLongPtr(window, GWLP_WNDPROC, (LONG_PTR)WndProc);
			ImGui::CreateContext();
			ImGuiIO& io = ImGui::GetIO();
			io.ConfigFlags = ImGuiConfigFlags_NoMouseCursorChange;

			D3DX11_IMAGE_LOAD_INFO info;
			ID3DX11ThreadPump* pump{ nullptr };
			D3DX11CreateShaderResourceViewFromMemory(p_device, attack_cursor, sizeof(attack_cursor), &info,
				pump, &attackCursor, nullptr);
			D3DX11CreateShaderResourceViewFromMemory(p_device, default_cursor, sizeof(default_cursor), &info,
				pump, &defaultCursor, nullptr);
			D3DX11CreateShaderResourceViewFromMemory(p_device, gui_cursor, sizeof(gui_cursor), &info,
				pump, &guiCursor, nullptr);
			D3DX11CreateShaderResourceViewFromMemory(p_device, pickup_cursor, sizeof(pickup_cursor), &info,
				pump, &pickupCursor, nullptr);
			D3DX11CreateShaderResourceViewFromMemory(p_device, target_cursor, sizeof(target_cursor), &info,
				pump, &targetCursor, nullptr);
			D3DX11CreateShaderResourceViewFromMemory(p_device, text_cursor, sizeof(text_cursor), &info,
				pump, &textCursor, nullptr);

			ImGui_ImplWin32_Init(window);
			ImGui_ImplDX11_Init(p_device, p_context);

			ImFont* font = io.Fonts->AddFontFromFileTTF(R"(C:\Windows\Fonts\segoeui.ttf)", 17.0f);
			hrFont = io.Fonts->AddFontFromFileTTF(R"(C:\Windows\Fonts\segoeui.ttf)", 20.0f);

			embraceTheDarkness();
			io.FontDefault = font;

			imguiinit = true;
		}
		else
			return p_present(p_swap_chain, sync_interval, flags);
	}

#pragma endregion

	ImGuiIO& io = ImGui::GetIO();

	if (GetAsyncKeyState(VK_INSERT) & 1) show = !show;

	if (show != lastShow)
	{
	}

	lastShow = show;

	if (lsWireframe != wireframe() || lsiWireframe != ignoreWireframe)
	{
		ImGui_ImplDX11_InvalidateDeviceObjects();
		CreateDeviceObjectsRemake();
	}

	lsiWireframe = ignoreWireframe;
	lsWireframe = wireframe();

	ImGui_ImplDX11_NewFrame();
	ImGui_ImplWin32_NewFrame();

	ImGui::NewFrame();

	if (showConsole)
		logs.Draw("Debug Logs");

	if (show)
	{
		ImGui::ShowDemoWindow();

		ImGui::Begin("Extender Menu", nullptr, 0);

		ImGui::BeginTabBar("TabBar");
		if (ImGui::TabItemButton("Info")) tab = 1;
		if (ImGui::TabItemButton("Utilities")) tab = 2;
		if (ImGui::TabItemButton("Toggles")) tab = 3;
		if (ImGui::TabItemButton("Extender Options")) tab = 4;
		if (ImGui::TabItemButton("Model Finder")) tab = 5;

		switch (tab)
		{
		case 1:
		{
			auto client = getClient();
			float cursorPos[3];
			cursorPos[0] = Read<float>((int)client + 0xEC);
			cursorPos[1] = Read<float>((int)client + 0xF0);
			cursorPos[2] = Read<float>((int)client + 0xF4);
			ImGui::Text("Cursor Position: %.2f, %.2f, %.2f", cursorPos[0], cursorPos[1], cursorPos[2]);
			auto ent = GetAddr(EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x0 });
			auto gent = (int)RTDynamicCast((uint32*)ent, 0, Entity, GameEntity, 0);
			setuptext"Entity" setupcontrol
				ImGui::Combo("", &selEntity, entities, IM_ARRAYSIZE(entities));
			setuptext"Position" setupcontrol
				ImGui::InputFloat3("\u200B", entPos, "%.2f", ImGuiInputTextFlags_ReadOnly);
			setuptext"Health" setupcontrol
				if (ImGui::SliderInt("\u200B\u200B", &HP, 0, 999))
				{
					Write<int>(gent + 0x24, HP);
				}
			setuptext"Action Points" setupcontrol
				if (ImGui::SliderInt("\u200B\u200B\u200B", &AP, 0, 100)) WritePtr<int>(
					EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x174 }, AP);

			ImGui::Text("Special: %d, %d, %d, %d, %d, %d, %d", Special[0], Special[1], Special[2], Special[3],
				Special[4], Special[5], Special[6]);
			ImGui::Text("Age: %d", Age);
			ImGui::Text("Level: %d", Level);
			ImGui::Text("Locked: %d", Locked);
			ImGui::Text("Race: %d", Race);
			ImGui::Text("Gender: %d", Gender);
			ImGui::Text("Ethnicity: %d", Ethnicity);
			ImGui::Text("SubRace: %d", SubRace);
			ImGui::Text("Type: %d", Type);
			ImGui::Text("LockDC: %d", LockDC);
			ImGui::Text("Active: %d", Active);
			auto entPtr = (const char*)GetAddr(EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x0 });
			const char* name; // eax
			const char* script; // eax
			const char* dialogue; // eax
			const char* filename; // eax
			const char* icon; // eax
			if (*((_DWORD*)entPtr + 0x76) < 0x10u)
				name = entPtr + 0x1C4;
			else
				name = (const char*)*((_DWORD*)entPtr + 0x71);
			ImGui::Text("Instance Name: %s", name);
			ImGui::Text("Entity ID #: %d", *((_DWORD*)entPtr + 0x77));
			if (*((_DWORD*)entPtr + 0xAE) < 0x10u)
				script = entPtr + 0x2A4;
			else
				script = (const char*)*((_DWORD*)entPtr + 0xA9);
			ImGui::Text("Script File name: %s", script);
			ImGui::Text("Team ID and Rank: %d and %d", *((_DWORD*)entPtr + 0xDA), *((_DWORD*)entPtr + 0xDB));
			ImGui::Text("Squad ID and Rank: %d and %d", *((_DWORD*)entPtr + 0xDC), *((_DWORD*)entPtr + 0xDD));
			if (*((_DWORD*)entPtr + 0x85) < 0x10u)
				dialogue = entPtr + 0x200;
			else
				dialogue = (const char*)*((_DWORD*)entPtr + 0x80);
			ImGui::Text("Dialogue File Name: %s", dialogue);
			if (*((_DWORD*)entPtr + 0x69) < 0x10u)
				filename = entPtr + 0x190;
			else
				filename = (const char*)*((_DWORD*)entPtr + 0x64);
			ImGui::Text("Entity File Name: %s", filename);
			if (*((_DWORD*)entPtr + 0x9A) < 0x10u)
				icon = entPtr + 0x254;
			else
				icon = (const char*)*((_DWORD*)entPtr + 0x95);
			ImGui::Text("Entity Icon File: %s", icon);
			ImGui::Text("Current Map ID: %p", *((_DWORD*)entPtr + 0x7B));
			ImGui::Text("Owner's ID: %d", *((_DWORD*)entPtr + 0xB7));
			if (*((_DWORD*)entPtr + 0x79))
				ImGui::Text("This entity is client controlled.");
			else
				ImGui::Text("This entity is not client controlled.");

			break;
		}
		case 2:
		{
			setuptext"Game Speed" setupcontrol
				if (ImGui::SliderFloat("", &speed, 0.5f, 10.0f, "%.1f")) setAllToSpeed(speed);
			break;
		}
		case 3:
		{
			const auto sz = ImVec2(ImGui::GetWindowWidth() / 3 - 10.66f, 0.0f);
			if (ImGui::Button("Statistics", sz)) ToggleStatistics();
			ImGui::SameLine();
			if (ImGui::Button("Lighting", sz)) ToggleLighting();
			ImGui::SameLine();
			if (ImGui::Button("Shadow", sz)) ToggleShadow();
			if (ImGui::Button("Camera Fog", sz)) ToggleCameraFog();
			ImGui::SameLine();
			if (ImGui::Button("Wireframe", sz)) ToggleWireframe();
			ImGui::SameLine();
			if (ImGui::Button("Transparency", sz)) ToggleTransparency();
			if (ImGui::Button("Octree Drawing", sz)) ToggleOctreeDrawing();
			ImGui::SameLine();
			if (ImGui::Button("Debug Light", sz)) DebugLight();
			ImGui::SameLine();
			if (ImGui::Button("Fog of War", sz)) ToggleFogOfWar();
			if (ImGui::Button("FOW Automap", sz)) ToggleFOWAutomap();
			ImGui::SameLine();
			if (ImGui::Button("FOW LOS", sz)) ToggleFOWLOS();
			ImGui::SameLine();
			if (ImGui::Button("Visual Effects", sz)) ToggleVisualEffects();
			if (ImGui::Button("Water", sz)) ToggleWater();
			ImGui::SameLine();
			if (ImGui::Button("Camera Constraints", sz)) CameraToggleConstraints();
			ImGui::SameLine();
			if (ImGui::Button("Debug Window", sz)) DebugWindow();
			if (ImGui::Button("Stack Window", sz)) StackWindow();
			ImGui::SameLine();
			if (ImGui::Button("Debug Tools", sz)) DebugTools();
			ImGui::SameLine();
			if (ImGui::Button("Scene", sz)) ToggleScene();
			if (ImGui::Button("Entities", sz)) ToggleEntities();
			ImGui::SameLine();
			if (ImGui::Button("GUI", sz)) ToggleGUI();
			ImGui::SameLine();
			if (ImGui::Button("Debug Logs", sz)) ToggleConsole();
			if (ImGui::Button("Rain", sz)) ToggleRain();
			ImGui::SameLine();
			if (ImGui::Button("Snow", sz)) ToggleSnow();
			ImGui::SameLine();
			if (ImGui::Button("Wind", sz)) ToggleWind();
			break;
		}
		case 4:
		{
			ImGui::Checkbox("Use Pickup Cursor", &usePickupCursor);
			ImGui::SameLine();
			HelpMarker("Toggles use of the hand cursor when moving items in inventory");
			ImGui::Checkbox("Ignore Wireframe State", &ignoreWireframe);
			ImGui::SameLine();
			HelpMarker("Toggles whether the extender menus will follow the game's wireframe state");
			ImGui::Checkbox("Use Entity Names", &useNames);
			ImGui::SameLine();
			HelpMarker("Toggles whether to use entity names instead of file names");
			ImGui::Checkbox("Show Mouseover Text", &showMouseOver);
			ImGui::SameLine();
			HelpMarker("Renders information about whatever the mouse cursor is over");
			ImGui::Checkbox("Alternate Statistics Menu", &altStats);
			ImGui::SameLine();
			HelpMarker("Puts different values in the statistics menu");
			break;
		}
		case 5:
		{
			ImGui::InputInt("ModelStride", &modelstride);
			ImGui::InputInt("ModelvscWidth", &modelvscwidth);
			ImGui::InputInt("ModelpscWidth", &modelpscwidth);
			ImGui::InputInt("ModelpsDescrFORMAT", &modelpsdescrformat);
			ImGui::Checkbox("Cycle Values", &cycle);
			if (cycle)
			{
				cycle_values();
			}
			break;
		}
		}

		ImGui::EndTabBar();

		ImGui::End();
	}

	const auto mp = ImGui::GetMousePos();
	const auto fdl = ImGui::GetForegroundDrawList();
	const auto bdl = ImGui::GetBackgroundDrawList();

	ID3D11ShaderResourceView* cursor = defaultCursor;
	auto size = ImVec2(27, 27);
	auto offset = ImVec2(3, 3);

	const auto cs1 = ReadPtr<int>(HudBase, { 0x2C, 0xA4, 0x28 });
	const auto cs2 = ReadPtr<int>(HudBase, { 0x2C, 0xA4, 0x30 });

	const bool mouseHidden = ReadPtr<byte>(HudBase, { 0x2C, 0xA4, 0x8, 0x38 });
	if (mouseHidden && !show) goto skipcursor;

	switch (cs1)
	{
	case 21:
		cursor = guiCursor;
		size = ImVec2(29, 29);
		offset = ImVec2(0, 0);
		break;
	case 18:
		cursor = textCursor;
		size = ImVec2(7, 15);
		offset = ImVec2(-4, -8);
		break;
	case 16:
		size = ImVec2(29, 29);
		offset = ImVec2(-14, -14);
		if (cs2 == 16) { cursor = targetCursor; }
		else if (cs2 == 15) { cursor = attackCursor; }
		else
		{
			size = ImVec2(30, 28);
			offset = ImVec2(2, 2);
			cursor = pickupCursor;
		}
		break;
	default:
		break;
	}

	if (cs2 == 14)
	{
		if (!usePickupCursor) goto skipcursor;
		cursor = pickupCursor;
		size = ImVec2(30, 28);
		offset = ImVec2(-8, 5);
	}

	fdl->AddImage(cursor, ImVec2(mp.x + offset.x, mp.y + offset.y),
		ImVec2(mp.x + size.x + offset.x, mp.y + size.y + offset.y), ImVec2(0, 0), ImVec2(1, 1), 0xFFFFFFFF);

skipcursor:

	if (showMouseOver || (altStats && renderStats))
	{
		const auto text = GetMouseOverText();
		if (showMouseOver)
		{
			OutlinedText(bdl, io.FontDefault, io.FontDefault->FontSize, ImVec2(mp.x, mp.y + 30), text);
		}

		if (altStats) OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 18), "Selected: %s", text);
	}

	const auto stats = ReadPtr<char>(HudBase, { 0x2C, 0x74, 0x4A });
	if (stats)
	{
		ToggleStatistics();
		renderStats = !renderStats;
	}

	if (renderStats)
	{
		if (altStats)
		{
			const auto Map = mapName();
			const auto FPS = Read<float>(SettingsBase + 0x6C4);
			const auto CamX = Read<float>(SettingsBase + 0xFC);
			const auto CamZ = Read<float>(SettingsBase + 0x100);
			const auto CamY = Read<float>(SettingsBase + 0x104);
			const auto camPtr = getCamPtr();
			const auto FOVy = camPtr[68] * RadToDeg;

			if (isValid(Map)) OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, -2), "Map: %s", Map);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 38), "FPS: %.1f (%.1f ms)", FPS, 1.0f / FPS * 1000);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 58), "Cam: (%.1f, %.1f, %.1f) %.1f FOV", CamX, CamZ, CamY, FOVy);
		}
		else
		{
			const auto Map = mapName();
			const auto FPS = Read<float>(SettingsBase + 0x6C4);
			const auto V = Read<int>(SettingsBase + 0x6D4);
			const auto T = Read<int>(SettingsBase + 0x6D0);
			const auto Vis = Read<int>(SettingsBase + 0x6F8);
			const auto Tex = Read<int>(SettingsBase + 0x6E4);
			const auto VB = Read<int>(SettingsBase + 0x6E8);
			const auto Mtx = Read<int>(SettingsBase + 0x6EC);
			const auto DIP = Read<int>(SettingsBase + 0x6F4);
			const auto DP = Read<int>(SettingsBase + 0x6F0);
			const auto Trans = ReadPtr<char>(SettingsBase, { 0x88D });
			const auto Light = ReadPtr<char>(SettingsBase, { 0x88E });
			const auto CamX = Read<float>(SettingsBase + 0xFC);
			const auto CamZ = Read<float>(SettingsBase + 0x100);
			const auto CamY = Read<float>(SettingsBase + 0x104);
			const auto camPtr = getCamPtr();
			const auto camPtr2 = getCamPtr2();
			const auto TargetX = camPtr2[36];
			const auto TargetZ = camPtr2[37];
			const auto TargetY = camPtr2[38];
			const auto Azi = camPtr2[39] * RadToDeg;
			const auto Elev = camPtr2[40] * RadToDeg;
			const auto Dist = camPtr2[41];
			const auto FOVy = camPtr[68] * RadToDeg;

			if (isValid(Map)) OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, -2), "Map: %s", Map);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 18), "FPS: %.1f V: %d T: %d Vis: %d", FPS, V, T, Vis);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 38), "Tex: %d VB: %d Mtx: %d DIP: %d DP: %d Trans: %s Light: %s",
				Tex, VB, Mtx, DIP, DP, Trans ? "On" : "Off", Light ? "On" : "Off");
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 58), "Cam: (%.1f, %.1f, %.1f)", CamX, CamZ, CamY);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 78), "Target: (%.1f, %.1f, %.1f)", TargetX, TargetZ, TargetY);
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 98), "Azi: %.1f Elev: %.1f Dist: %.1f FOVy: %.1f", Azi, Elev,
				Dist, FOVy);

			// Set up camera transform
			DirectX::XMFLOAT3 cameraPos(CamX, CamY, CamZ);
			DirectX::XMFLOAT3 cameraTarget(TargetX, TargetY, TargetZ);
			DirectX::XMFLOAT3 cameraUp(0.0f, 0.0f, 1.0f);
			DirectX::XMMATRIX viewMatrix = DirectX::XMMatrixLookAtLH(DirectX::XMLoadFloat3(&cameraPos), DirectX::XMLoadFloat3(&cameraTarget), DirectX::XMLoadFloat3(&cameraUp));

			// Set up projection transform
			float aspectRatio = ImGui::GetIO().DisplaySize.x / ImGui::GetIO().DisplaySize.y;
			float nearZ = 1.0f;
			float farZ = 10000.0f;
			float fovY = camPtr[68];
			float height = 2.0f * nearZ * tanf(fovY / 2.0f);
			float width = height * aspectRatio;
			DirectX::XMMATRIX projMatrix = DirectX::XMMatrixPerspectiveLH(width, height, nearZ, farZ);

			// Combine view and projection matrices
			DirectX::XMMATRIX viewProjMatrix = viewMatrix * projMatrix;

			// Apply camera rotations
			DirectX::XMMATRIX rotationMatrix = DirectX::XMMatrixRotationRollPitchYaw(camPtr2[40], camPtr2[39], 0.0f);
			viewProjMatrix = viewProjMatrix * rotationMatrix;

			// Set up world-space coordinates
			float worldX = 10.0f;
			float worldY = 0.0f;
			float worldZ = 10.0f;

			// Transform world-space coordinates to screen-space coordinates
			DirectX::XMVECTOR worldPos = DirectX::XMVectorSet(worldX, worldY, worldZ, 1.0f);
			DirectX::XMVECTOR screenPos = DirectX::XMVector4Transform(worldPos, viewProjMatrix);

			// Extract screen-space coordinates
			float screenX = DirectX::XMVectorGetX(screenPos) / DirectX::XMVectorGetW(screenPos);
			float screenY = DirectX::XMVectorGetY(screenPos) / DirectX::XMVectorGetW(screenPos);

			bdl->AddImage(targetCursor, ImVec2(screenX - 14, screenY - 14),
				ImVec2(screenX + 15, screenY + 15), ImVec2(0, 0), ImVec2(1, 1), 0xFFFFFFFF);
		}
	}

	ImGui::Render();

	p_context->OMSetRenderTargets(1, &mainRenderTargetView, nullptr);
	ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());

	return p_present(p_swap_chain, sync_interval, flags);
}
