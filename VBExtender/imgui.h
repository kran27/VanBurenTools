#pragma once
#define WIN32_LEAN_AND_MEAN

#pragma comment(lib, "D3DX11.lib")
#pragma comment(lib, "d3d11.lib")
#include <iostream>

#include <windows.h>
#include <ctype.h>
#include <d3dcompiler.h>
#include <d3d11.h>
#include <D3DX11tex.h>

#include "ImGui/imgui.h"
#include "ImGui/imgui_impl_win32.h"
#include "ImGui/imgui_impl_dx11.h"
#include "byte_arrays.h"
#include "globals.h"
#include "functions.h"
#include "memory.h"
#include "speed.h"

#define setuptext ImGui::SetCursorPosY(ImGui::GetCursorPosY() + 3); ImGui::SetCursorPosX(12); ImGui::Text(
#define setupcontrol ); ImGui::SameLine(); ImGui::SetCursorPosY(ImGui::GetCursorPosY() - 3); ImGui::SetCursorPosX(100); ImGui::SetNextItemWidth(ImGui::GetContentRegionAvail().x);

#pragma region init/setup
struct ImGui_ImplDX11_Data;
HINSTANCE dll_handle;

typedef long(__stdcall* present)(IDXGISwapChain*, UINT, UINT);
present p_present;

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
		return true;
	}
	return false;
}

WNDPROC oWndProc;
extern LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
LRESULT __stdcall WndProc(const HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
	if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
		return true;

	return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
}

void embraceTheDarkness()
{
	ImVec4* colors = ImGui::GetStyle().Colors;
	colors[ImGuiCol_Text] = ImVec4(1.00f, 1.00f, 1.00f, 1.00f);
	colors[ImGuiCol_TextDisabled] = ImVec4(0.50f, 0.50f, 0.50f, 1.00f);
	colors[ImGuiCol_WindowBg] = ImVec4(0.10f, 0.10f, 0.10f, 0.90f);
	colors[ImGuiCol_ChildBg] = ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
	colors[ImGuiCol_PopupBg] = ImVec4(0.19f, 0.19f, 0.19f, 0.92f);
	colors[ImGuiCol_Border] = ImVec4(0.19f, 0.19f, 0.19f, 0.29f);
	colors[ImGuiCol_BorderShadow] = ImVec4(0.00f, 0.00f, 0.00f, 0.24f);
	colors[ImGuiCol_FrameBg] = ImVec4(0.05f, 0.05f, 0.05f, 0.54f);
	colors[ImGuiCol_FrameBgHovered] = ImVec4(0.19f, 0.19f, 0.19f, 0.54f);
	colors[ImGuiCol_FrameBgActive] = ImVec4(0.20f, 0.22f, 0.23f, 1.00f);
	colors[ImGuiCol_TitleBg] = ImVec4(0.05f, 0.05f, 0.05f, 0.90f);
	colors[ImGuiCol_TitleBgActive] = ImVec4(0.00f, 0.00f, 0.00f, 0.90f);
	colors[ImGuiCol_TitleBgCollapsed] = ImVec4(0.00f, 0.00f, 0.00f, 0.90f);
	colors[ImGuiCol_MenuBarBg] = ImVec4(0.14f, 0.14f, 0.14f, 0.80f);
	colors[ImGuiCol_ScrollbarBg] = ImVec4(0.05f, 0.05f, 0.05f, 0.54f);
	colors[ImGuiCol_ScrollbarGrab] = ImVec4(0.34f, 0.34f, 0.34f, 0.54f);
	colors[ImGuiCol_ScrollbarGrabHovered] = ImVec4(0.40f, 0.40f, 0.40f, 0.54f);
	colors[ImGuiCol_ScrollbarGrabActive] = ImVec4(0.56f, 0.56f, 0.56f, 0.54f);
	colors[ImGuiCol_CheckMark] = ImVec4(0.33f, 0.67f, 0.86f, 1.00f);
	colors[ImGuiCol_SliderGrab] = ImVec4(0.34f, 0.34f, 0.34f, 0.54f);
	colors[ImGuiCol_SliderGrabActive] = ImVec4(0.56f, 0.56f, 0.56f, 0.54f);
	colors[ImGuiCol_Button] = ImVec4(0.05f, 0.05f, 0.05f, 0.54f);
	colors[ImGuiCol_ButtonHovered] = ImVec4(0.19f, 0.19f, 0.19f, 0.54f);
	colors[ImGuiCol_ButtonActive] = ImVec4(0.20f, 0.22f, 0.23f, 1.00f);
	colors[ImGuiCol_Header] = ImVec4(0.00f, 0.00f, 0.00f, 0.52f);
	colors[ImGuiCol_HeaderHovered] = ImVec4(0.00f, 0.00f, 0.00f, 0.36f);
	colors[ImGuiCol_HeaderActive] = ImVec4(0.20f, 0.22f, 0.23f, 0.33f);
	colors[ImGuiCol_Separator] = ImVec4(0.28f, 0.28f, 0.28f, 0.29f);
	colors[ImGuiCol_SeparatorHovered] = ImVec4(0.44f, 0.44f, 0.44f, 0.29f);
	colors[ImGuiCol_SeparatorActive] = ImVec4(0.40f, 0.44f, 0.47f, 1.00f);
	colors[ImGuiCol_ResizeGrip] = ImVec4(0.28f, 0.28f, 0.28f, 0.29f);
	colors[ImGuiCol_ResizeGripHovered] = ImVec4(0.44f, 0.44f, 0.44f, 0.29f);
	colors[ImGuiCol_ResizeGripActive] = ImVec4(0.40f, 0.44f, 0.47f, 1.00f);
	colors[ImGuiCol_Tab] = ImVec4(0.00f, 0.00f, 0.00f, 0.52f);
	colors[ImGuiCol_TabHovered] = ImVec4(0.14f, 0.14f, 0.14f, 1.00f);
	colors[ImGuiCol_TabActive] = ImVec4(0.20f, 0.20f, 0.20f, 0.36f);
	colors[ImGuiCol_TabUnfocused] = ImVec4(0.00f, 0.00f, 0.00f, 0.52f);
	colors[ImGuiCol_TabUnfocusedActive] = ImVec4(0.14f, 0.14f, 0.14f, 1.00f);
	colors[ImGuiCol_PlotLines] = ImVec4(1.00f, 0.00f, 0.00f, 1.00f);
	colors[ImGuiCol_PlotLinesHovered] = ImVec4(1.00f, 0.00f, 0.00f, 1.00f);
	colors[ImGuiCol_PlotHistogram] = ImVec4(1.00f, 0.00f, 0.00f, 1.00f);
	colors[ImGuiCol_PlotHistogramHovered] = ImVec4(1.00f, 0.00f, 0.00f, 1.00f);
	colors[ImGuiCol_TableHeaderBg] = ImVec4(0.00f, 0.00f, 0.00f, 0.52f);
	colors[ImGuiCol_TableBorderStrong] = ImVec4(0.00f, 0.00f, 0.00f, 0.52f);
	colors[ImGuiCol_TableBorderLight] = ImVec4(0.28f, 0.28f, 0.28f, 0.29f);
	colors[ImGuiCol_TableRowBg] = ImVec4(0.00f, 0.00f, 0.00f, 0.00f);
	colors[ImGuiCol_TableRowBgAlt] = ImVec4(1.00f, 1.00f, 1.00f, 0.06f);
	colors[ImGuiCol_TextSelectedBg] = ImVec4(0.20f, 0.22f, 0.23f, 1.00f);
	colors[ImGuiCol_DragDropTarget] = ImVec4(0.33f, 0.67f, 0.86f, 1.00f);
	colors[ImGuiCol_NavHighlight] = ImVec4(1.00f, 0.00f, 0.00f, 1.00f);
	colors[ImGuiCol_NavWindowingHighlight] = ImVec4(1.00f, 0.00f, 0.00f, 0.70f);
	colors[ImGuiCol_NavWindowingDimBg] = ImVec4(1.00f, 0.00f, 0.00f, 0.20f);
	colors[ImGuiCol_ModalWindowDimBg] = ImVec4(1.00f, 0.00f, 0.00f, 0.35f);

	ImGuiStyle& style = ImGui::GetStyle();
	style.ScrollbarSize = 15;
	style.WindowRounding = 7;
	style.ChildRounding = 4;
	style.FrameRounding = 3;
	style.PopupRounding = 4;
	style.ScrollbarRounding = 9;
	style.GrabRounding = 3;
	style.TabRounding = 4;
	style.WindowBorderSize = 1.0f;
}

static void HelpMarker(const char* desc)
{
	ImGui::TextDisabled("(?)");
	if (ImGui::IsItemHovered(ImGuiHoveredFlags_DelayShort))
	{
		ImGui::BeginTooltip();
		ImGui::PushTextWrapPos(ImGui::GetFontSize() * 35.0f);
		ImGui::TextUnformatted(desc);
		ImGui::PopTextWrapPos();
		ImGui::EndTooltip();
	}
}

#pragma endregion

bool show = false;
bool lastShow;
bool imguiinit = false;
HWND window = NULL;
uint tab = 1;
ID3D11Device* p_device = NULL;
ID3D11DeviceContext* p_context = NULL;
ID3D11RenderTargetView* mainRenderTargetView = NULL;
ID3D11ShaderResourceView* attackCursor = nullptr;
ID3D11ShaderResourceView* defaultCursor = nullptr;
ID3D11ShaderResourceView* guiCursor = nullptr;
ID3D11ShaderResourceView* pickupCursor = nullptr;
ID3D11ShaderResourceView* targetCursor = nullptr;
ID3D11ShaderResourceView* textCursor = nullptr;

bool usePickupCursor = false;
inline bool wireframe() { return ReadPtr<char>(HudBase, { 0x2C, 0x74, 0x58 }) == 1; }
bool lsWireframe = false;
bool ignoreWireframe = false;
bool lsiWireframe = false;
bool showMouseOver = false;
float speed = 1.0f;
bool renderStats = false;
bool altStats = false;

inline void DrawGOBJData(uint32* ent)
{
	auto gobj = (int)RTDynamicCast(ent, 0, Entity, GameUsableObject, 0);
}

inline void DrawGITMData(uint32* ent)
{
	auto gitm = (int)RTDynamicCast((uint32*)ent, 0, Entity, GameItem, 0);
}

#pragma region Stuff before render code

struct VERTEX_CONSTANT_BUFFER_DX11
{
	float   mvp[4][4];
};

struct ImGui_ImplDX11_Data
{
	ID3D11Device* pd3dDevice;
	ID3D11DeviceContext* pd3dDeviceContext;
	IDXGIFactory* pFactory;
	ID3D11Buffer* pVB;
	ID3D11Buffer* pIB;
	ID3D11VertexShader* pVertexShader;
	ID3D11InputLayout* pInputLayout;
	ID3D11Buffer* pVertexConstantBuffer;
	ID3D11PixelShader* pPixelShader;
	ID3D11SamplerState* pFontSampler;
	ID3D11ShaderResourceView* pFontTextureView;
	ID3D11RasterizerState* pRasterizerState;
	ID3D11BlendState* pBlendState;
	ID3D11DepthStencilState* pDepthStencilState;
	int                         VertexBufferSize;
	int                         IndexBufferSize;

	ImGui_ImplDX11_Data() { memset((void*)this, 0, sizeof(*this)); VertexBufferSize = 5000; IndexBufferSize = 10000; }
};

static void ImGui_ImplDX11_CreateFontsTexture()
{
	// Build texture atlas
	ImGuiIO& io = ImGui::GetIO();
	auto bd = (ImGui_ImplDX11_Data*)io.BackendRendererUserData;
	unsigned char* pixels;
	int width, height;
	io.Fonts->GetTexDataAsRGBA32(&pixels, &width, &height);

	// Upload texture to graphics system
	{
		D3D11_TEXTURE2D_DESC desc;
		ZeroMemory(&desc, sizeof(desc));
		desc.Width = width;
		desc.Height = height;
		desc.MipLevels = 1;
		desc.ArraySize = 1;
		desc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		desc.SampleDesc.Count = 1;
		desc.Usage = D3D11_USAGE_DEFAULT;
		desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
		desc.CPUAccessFlags = 0;

		ID3D11Texture2D* pTexture = nullptr;
		D3D11_SUBRESOURCE_DATA subResource;
		subResource.pSysMem = pixels;
		subResource.SysMemPitch = desc.Width * 4;
		subResource.SysMemSlicePitch = 0;
		bd->pd3dDevice->CreateTexture2D(&desc, &subResource, &pTexture);
		IM_ASSERT(pTexture != nullptr);

		// Create texture view
		D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc;
		ZeroMemory(&srvDesc, sizeof(srvDesc));
		srvDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		srvDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
		srvDesc.Texture2D.MipLevels = desc.MipLevels;
		srvDesc.Texture2D.MostDetailedMip = 0;
		bd->pd3dDevice->CreateShaderResourceView(pTexture, &srvDesc, &bd->pFontTextureView);
		pTexture->Release();
	}

	// Store our identifier
	io.Fonts->SetTexID((ImTextureID)bd->pFontTextureView);

	// Create texture sampler
	// (Bilinear sampling is required by default. Set 'io.Fonts->Flags |= ImFontAtlasFlags_NoBakedLines' or 'style.AntiAliasedLinesUseTex = false' to allow point/nearest sampling)
	{
		D3D11_SAMPLER_DESC desc;
		ZeroMemory(&desc, sizeof(desc));
		desc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
		desc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
		desc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
		desc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
		desc.MipLODBias = 0.f;
		desc.ComparisonFunc = D3D11_COMPARISON_ALWAYS;
		desc.MinLOD = 0.f;
		desc.MaxLOD = 0.f;
		bd->pd3dDevice->CreateSamplerState(&desc, &bd->pFontSampler);
	}
}

bool CreateDeviceObjectsRemake() {
	ImGuiIO& io = ImGui::GetIO();

	auto bd = (ImGui_ImplDX11_Data*)io.BackendRendererUserData;
	if (!bd->pd3dDevice)
		return false;
	if (bd->pFontSampler)
		ImGui_ImplDX11_InvalidateDeviceObjects();

	// By using D3DCompile() from <d3dcompiler.h> / d3dcompiler.lib, we introduce a dependency to a given version of d3dcompiler_XX.dll (see D3DCOMPILER_DLL_A)
	// If you would like to use this DX11 sample code but remove this dependency you can:
	//  1) compile once, save the compiled shader blobs into a file or source code and pass them to CreateVertexShader()/CreatePixelShader() [preferred solution]
	//  2) use code to detect any version of the DLL and grab a pointer to D3DCompile from the DLL.
	// See https://github.com/ocornut/imgui/pull/638 for sources and details.

	// Create the vertex shader
	{
		static const char* vertexShader =
			"cbuffer vertexBuffer : register(b0) \
            {\
              float4x4 ProjectionMatrix; \
            };\
            struct VS_INPUT\
            {\
              float2 pos : POSITION;\
              float4 col : COLOR0;\
              float2 uv  : TEXCOORD0;\
            };\
            \
            struct PS_INPUT\
            {\
              float4 pos : SV_POSITION;\
              float4 col : COLOR0;\
              float2 uv  : TEXCOORD0;\
            };\
            \
            PS_INPUT main(VS_INPUT input)\
            {\
              PS_INPUT output;\
              output.pos = mul( ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));\
              output.col = input.col;\
              output.uv  = input.uv;\
              return output;\
            }";

		ID3DBlob* vertexShaderBlob;
		if (FAILED(D3DCompile(vertexShader, strlen(vertexShader), nullptr, nullptr, nullptr, "main", "vs_4_0", 0, 0, &vertexShaderBlob, nullptr)))
			return false; // NB: Pass ID3DBlob* pErrorBlob to D3DCompile() to get error showing in (const char*)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
		if (bd->pd3dDevice->CreateVertexShader(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), nullptr, &bd->pVertexShader) != S_OK)
		{
			vertexShaderBlob->Release();
			return false;
		}

		// Create the input layout
		D3D11_INPUT_ELEMENT_DESC local_layout[] =
		{
			{ "POSITION", 0, DXGI_FORMAT_R32G32_FLOAT,   0, (UINT)IM_OFFSETOF(ImDrawVert, pos), D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,   0, (UINT)IM_OFFSETOF(ImDrawVert, uv),  D3D11_INPUT_PER_VERTEX_DATA, 0 },
			{ "COLOR",    0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, (UINT)IM_OFFSETOF(ImDrawVert, col), D3D11_INPUT_PER_VERTEX_DATA, 0 },
		};
		if (bd->pd3dDevice->CreateInputLayout(local_layout, 3, vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), &bd->pInputLayout) != S_OK)
		{
			vertexShaderBlob->Release();
			return false;
		}
		vertexShaderBlob->Release();

		// Create the constant buffer
		{
			D3D11_BUFFER_DESC desc;
			desc.ByteWidth = sizeof(VERTEX_CONSTANT_BUFFER_DX11);
			desc.Usage = D3D11_USAGE_DYNAMIC;
			desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
			desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
			desc.MiscFlags = 0;
			bd->pd3dDevice->CreateBuffer(&desc, nullptr, &bd->pVertexConstantBuffer);
		}
	}

	// Create the pixel shader
	{
		static const char* pixelShader =
			"struct PS_INPUT\
            {\
            float4 pos : SV_POSITION;\
            float4 col : COLOR0;\
            float2 uv  : TEXCOORD0;\
            };\
            sampler sampler0;\
            Texture2D texture0;\
            \
            float4 main(PS_INPUT input) : SV_Target\
            {\
            float4 out_col = input.col * texture0.Sample(sampler0, input.uv); \
            return out_col; \
            }";

		ID3DBlob* pixelShaderBlob;
		if (FAILED(D3DCompile(pixelShader, strlen(pixelShader), nullptr, nullptr, nullptr, "main", "ps_4_0", 0, 0, &pixelShaderBlob, nullptr)))
			return false; // NB: Pass ID3DBlob* pErrorBlob to D3DCompile() to get error showing in (const char*)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
		if (bd->pd3dDevice->CreatePixelShader(pixelShaderBlob->GetBufferPointer(), pixelShaderBlob->GetBufferSize(), nullptr, &bd->pPixelShader) != S_OK)
		{
			pixelShaderBlob->Release();
			return false;
		}
		pixelShaderBlob->Release();
	}

	// Create the blending setup
	{
		D3D11_BLEND_DESC desc;
		ZeroMemory(&desc, sizeof(desc));
		desc.AlphaToCoverageEnable = false;
		desc.RenderTarget[0].BlendEnable = true;
		desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
		desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
		desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
		desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
		desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
		desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
		desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
		p_device->CreateBlendState(&desc, &bd->pBlendState);
	}

	// Create the rasterizer state
	{
		D3D11_RASTERIZER_DESC desc;
		ZeroMemory(&desc, sizeof(desc));
		if (ignoreWireframe || !wireframe()) desc.FillMode = D3D11_FILL_SOLID;
		else desc.FillMode = D3D11_FILL_WIREFRAME;
		desc.CullMode = D3D11_CULL_NONE;
		desc.ScissorEnable = true;
		desc.DepthClipEnable = true;
		p_device->CreateRasterizerState(&desc, &bd->pRasterizerState);
	}

	// Create depth-stencil State
	{
		D3D11_DEPTH_STENCIL_DESC desc;
		ZeroMemory(&desc, sizeof(desc));
		desc.DepthEnable = false;
		desc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
		desc.DepthFunc = D3D11_COMPARISON_ALWAYS;
		desc.StencilEnable = false;
		desc.FrontFace.StencilFailOp = desc.FrontFace.StencilDepthFailOp = desc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
		desc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;
		desc.BackFace = desc.FrontFace;
		bd->pd3dDevice->CreateDepthStencilState(&desc, &bd->pDepthStencilState);
	}

	ImGui_ImplDX11_CreateFontsTexture();
	return true;
}

inline void OutlinedText(ImDrawList* dl, const ImFont* fnt, const float sz, const ImVec2 pos, char const* fmt, ...)
{
	char text[256];
	va_list args;
	va_start(args, fmt);
	vsprintf_s(text, fmt, args);
	va_end(args);

	dl->AddText(fnt, sz, ImVec2(pos.x - 1, pos.y - 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x, pos.y - 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x + 1, pos.y - 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x - 1, pos.y + 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x, pos.y + 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x + 1, pos.y + 1), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x - 1, pos.y), 0xFF000000, text);
	dl->AddText(fnt, sz, ImVec2(pos.x + 1, pos.y), 0xFF000000, text);
	dl->AddText(fnt, sz, pos, 0xFFFFFFFF, text);
}

ImFont* hrFont;
static long __stdcall detour_present(IDXGISwapChain* p_swap_chain, UINT sync_interval, UINT flags) {
	if (!imguiinit) {
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

	if (show != lastShow) {}

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

	if (show) {
		ImGui::ShowDemoWindow();
		ImGui::Begin("Extender Menu", nullptr, 0);

		ImGui::BeginTabBar("TabBar");
		if (ImGui::TabItemButton("Info")) tab = 1;
		if (ImGui::TabItemButton("Utilities")) tab = 2;
		if (ImGui::TabItemButton("Toggles")) tab = 3;
		if (ImGui::TabItemButton("Extender Options")) tab = 4;

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
			setuptext "Entity" setupcontrol
				ImGui::Combo("", &selEntity, entities, IM_ARRAYSIZE(entities));
			setuptext "Position" setupcontrol
				ImGui::InputFloat3("\u200B", entPos, "%.2f", ImGuiInputTextFlags_ReadOnly);
			setuptext "Health" setupcontrol
				if (ImGui::SliderInt("\u200B\u200B", &HP, 0, 999)) {
					Write<int>(gent + 0x24, HP);
				}
			setuptext "Action Points" setupcontrol
				if (ImGui::SliderInt("\u200B\u200B\u200B", &AP, 0, 100)) WritePtr<int>(EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x174 }, AP);

			ImGui::Text("Special: %d, %d, %d, %d, %d, %d, %d", Special[0], Special[1], Special[2], Special[3], Special[4], Special[5], Special[6]);
			ImGui::Text("Age: %d", Age);

			//if (ImGui::Button("Kill")){
			//	auto gent = GetAddr(EntityBase, { 0xE8, (uint)selEntity * 4, 0x2C, 0x0 });
			//	auto ent = (uint)RTDynamicCast((uint32*)gent, 0, Entity, GameEntity, 0);
			//	auto entID = ReadPtr<int>(ent + 0x4, {0x4, ent + 0x1E0});
			//	auto client = gameClient();
			//	Kill((uintptr_t)client, entID);
			//}
			break;
		}
		case 2:
		{
			setuptext "Game Speed" setupcontrol
				if (ImGui::SliderFloat("", &speed, 0.5f, 10.0f, "%.1f")) setAllToSpeed(speed);
			break;
		}
		case 3:
		{
			const auto sz = ImVec2(ImGui::GetWindowWidth() / 3 - 10.66f, 0.0f);
			if (ImGui::Button("Statistics", sz)) ToggleStatistics(); ImGui::SameLine();
			if (ImGui::Button("Lighting", sz)) ToggleLighting(); ImGui::SameLine();
			if (ImGui::Button("Shadow", sz)) ToggleShadow();
			if (ImGui::Button("Camera Fog", sz)) ToggleCameraFog(); ImGui::SameLine();
			if (ImGui::Button("Wireframe", sz)) ToggleWireframe(); ImGui::SameLine();
			if (ImGui::Button("Transparency", sz)) ToggleTransparency();
			if (ImGui::Button("Octree Drawing", sz)) ToggleOctreeDrawing(); ImGui::SameLine();
			if (ImGui::Button("Debug Light", sz)) DebugLight(); ImGui::SameLine();
			if (ImGui::Button("Fog of War", sz)) ToggleFogOfWar();
			if (ImGui::Button("FOW Automap", sz)) ToggleFOWAutomap(); ImGui::SameLine();
			if (ImGui::Button("FOW LOS", sz)) ToggleFOWLOS(); ImGui::SameLine();
			if (ImGui::Button("Visual Effects", sz)) ToggleVisualEffects();
			if (ImGui::Button("Water", sz)) ToggleWater(); ImGui::SameLine();
			if (ImGui::Button("Camera Constraints", sz)) CameraToggleConstraints(); ImGui::SameLine();
			if (ImGui::Button("Debug Window", sz)) DebugWindow();
			if (ImGui::Button("Stack Window", sz)) StackWindow(); ImGui::SameLine();
			if (ImGui::Button("Debug Tools", sz)) DebugTools(); ImGui::SameLine();
			if (ImGui::Button("Scene", sz)) ToggleScene();
			if (ImGui::Button("Entities", sz)) ToggleEntities(); ImGui::SameLine();
			if (ImGui::Button("GUI", sz)) ToggleGUI(); ImGui::SameLine();
			if (ImGui::Button("Debug Log", sz)) ToggleConsole();
			if (ImGui::Button("Rain", sz)) ToggleRain(); ImGui::SameLine();
			if (ImGui::Button("Snow", sz)) ToggleSnow(); ImGui::SameLine();
			if (ImGui::Button("Wind", sz)) ToggleWind();
			break;
		}
		case 4:
			ImGui::Checkbox("Use Pickup Cursor", &usePickupCursor); ImGui::SameLine();
			HelpMarker("Toggles use of the hand cursor when moving items in inventory");
			ImGui::Checkbox("Ignore Wireframe State", &ignoreWireframe); ImGui::SameLine();
			HelpMarker("Toggles whether the extender menus will follow the game's wireframe state");
			ImGui::Checkbox("Use Entity Names", &useNames); ImGui::SameLine();
			HelpMarker("Toggles whether to use entity names instead of file names");
			ImGui::Checkbox("Show Mouseover Text", &showMouseOver); ImGui::SameLine();
			HelpMarker("Renders information about whatever the mouse cursor is over");
			ImGui::Checkbox("Alternate Statistics Menu", &altStats); ImGui::SameLine();
			HelpMarker("Puts different values in the statistics menu");
			break;
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
		cursor = guiCursor; size = ImVec2(29, 29); offset = ImVec2(0, 0);
		break;
	case 18:
		cursor = textCursor; size = ImVec2(7, 15); offset = ImVec2(-4, -8);
		break;
	case 16:
		size = ImVec2(29, 29); offset = ImVec2(-14, -14);
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
		cursor = pickupCursor; size = ImVec2(30, 28); offset = ImVec2(-8, 5);
	}

	fdl->AddImage(cursor, ImVec2(mp.x + offset.x, mp.y + offset.y), ImVec2(mp.x + size.x + offset.x, mp.y + size.y + offset.y), ImVec2(0, 0), ImVec2(1, 1), 0xFFFFFFFF);

skipcursor:
	if (showMouseOver || (altStats && renderStats))
	{
		const auto text = GetMouseOverText();
		if (showMouseOver) {
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
		else {
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
			OutlinedText(bdl, hrFont, 20.0f, ImVec2(2, 98), "Azi: %.1f Elev: %.1f Dist: %.1f FOVy: %.1f", Azi, Elev, Dist, FOVy);
		}
	}

	ImGui::Render();

	p_context->OMSetRenderTargets(1, &mainRenderTargetView, nullptr);
	ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());

	return p_present(p_swap_chain, sync_interval, flags);
}