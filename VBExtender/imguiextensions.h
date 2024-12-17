#pragma once
#include <d3d11.h>
#include <d3dcompiler.h>
#include <stdio.h>
#include "imgui.h"
#pragma comment(lib, "D3DX11.lib")
#pragma comment(lib, "d3d11.lib")
#include <iostream>

#include <windows.h>
#include <ctype.h>
#include <d3dcompiler.h>
#include <d3d11.h>
#include <D3DX11tex.h>

#include "backends/imgui_impl_win32.h"
#include "backends/imgui_impl_dx11.h"
#include "byte_arrays.h"
#include "globals.h"
#include "functions.h"
#include "memory.h"
#include "speed.h"

#include <DirectXMath.h>

ID3D11Device* p_device = NULL;

struct VERTEX_CONSTANT_BUFFER_DX11
{
	float mvp[4][4];
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
	int VertexBufferSize;
	int IndexBufferSize;

	ImGui_ImplDX11_Data()
	{
		memset((void*)this, 0, sizeof(*this));
		VertexBufferSize = 5000;
		IndexBufferSize = 10000;
	}
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

inline bool wireframe() { return ReadPtr<char>(HudBase, { 0x2C, 0x74, 0x58 }) == 1; }
bool lsWireframe = false;
bool ignoreWireframe = false;
bool lsiWireframe = false;

bool CreateDeviceObjectsRemake()
{
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
		if (FAILED(
			D3DCompile(vertexShader, strlen(vertexShader), nullptr, nullptr, nullptr, "main", "vs_4_0", 0, 0, &
				vertexShaderBlob, nullptr)))
			return false;
		// NB: Pass ID3DBlob* pErrorBlob to D3DCompile() to get error showing in (const char*)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
		if (bd->pd3dDevice->CreateVertexShader(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(),
			nullptr, &bd->pVertexShader) != S_OK)
		{
			vertexShaderBlob->Release();
			return false;
		}

		// Create the input layout
		D3D11_INPUT_ELEMENT_DESC local_layout[] =
		{
			{
				"POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0, (UINT)IM_OFFSETOF(ImDrawVert, pos),
				D3D11_INPUT_PER_VERTEX_DATA, 0
			},
			{
				"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, (UINT)IM_OFFSETOF(ImDrawVert, uv),
				D3D11_INPUT_PER_VERTEX_DATA, 0
			},
			{
				"COLOR", 0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, (UINT)IM_OFFSETOF(ImDrawVert, col),
				D3D11_INPUT_PER_VERTEX_DATA, 0
			},
		};
		if (bd->pd3dDevice->CreateInputLayout(local_layout, 3, vertexShaderBlob->GetBufferPointer(),
			vertexShaderBlob->GetBufferSize(), &bd->pInputLayout) != S_OK)
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
		if (FAILED(
			D3DCompile(pixelShader, strlen(pixelShader), nullptr, nullptr, nullptr, "main", "ps_4_0", 0, 0, &
				pixelShaderBlob, nullptr)))
			return false;
		// NB: Pass ID3DBlob* pErrorBlob to D3DCompile() to get error showing in (const char*)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
		if (bd->pd3dDevice->CreatePixelShader(pixelShaderBlob->GetBufferPointer(), pixelShaderBlob->GetBufferSize(),
			nullptr, &bd->pPixelShader) != S_OK)
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
		desc.FrontFace.StencilFailOp = desc.FrontFace.StencilDepthFailOp = desc.FrontFace.StencilPassOp =
			D3D11_STENCIL_OP_KEEP;
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

void embraceTheDarkness()
{
	ImVec4* colors = ImGui::GetStyle().Colors;
	colors[0] = ImVec4(0.8352941f, 0.8352941f, 0.8352941f, 1.0f);
	colors[1] = ImVec4(0.39607844f, 0.39607844f, 0.39607844f, 1.0f);
	colors[2] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[3] = ImVec4(1.0f, 0.0f, 0.0f, 1.0f);
	colors[4] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[5] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);
	colors[6] = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);
	colors[7] = ImVec4(0.101960786f, 0.101960786f, 0.10980392f, 1.0f);
	colors[8] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);
	colors[9] = ImVec4(0.2f, 0.22f, 0.23f, 1.0f);
	colors[10] = ImVec4(0.101960786f, 0.101960786f, 0.10980392f, 1.0f);
	colors[11] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[12] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[13] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[14] = ImVec4(0.101960786f, 0.101960786f, 0.10980392f, 1.0f);
	colors[15] = ImVec4(0.34f, 0.34f, 0.34f, 1.0f);
	colors[16] = ImVec4(0.4f, 0.4f, 0.4f, 1.0f);
	colors[17] = ImVec4(0.56f, 0.56f, 0.56f, 1.0f);
	colors[18] = ImVec4(0.8352941f, 0.8352941f, 0.8352941f, 1.0f);
	colors[19] = ImVec4(0.34f, 0.34f, 0.34f, 1.0f);
	colors[20] = ImVec4(0.56f, 0.56f, 0.56f, 1.0f);
	colors[21] = ImVec4(0.101960786f, 0.101960786f, 0.10980392f, 1.0f);
	colors[22] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);
	colors[23] = ImVec4(0.2f, 0.22f, 0.23f, 1.0f);
	colors[24] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[25] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[26] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[27] = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);
	colors[28] = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);
	colors[29] = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);
	colors[30] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);
	colors[31] = ImVec4(0.44f, 0.44f, 0.44f, 1.0f);
	colors[32] = ImVec4(0.4f, 0.44f, 0.47f, 1.0f);
	colors[33] = ImVec4(0.101960786f, 0.101960786f, 0.10980392f, 1.0f);
	colors[34] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[35] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[36] = ImVec4(0.08627451f, 0.08627451f, 0.08627451f, 1.0f);
	colors[37] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[38] = ImVec4(0.26f, 0.59f, 0.98f, 0.7f);
	colors[39] = ImVec4(0.2f, 0.2f, 0.2f, 1.0f);
	colors[40] = ImVec4(0.0627451f, 0.0627451f, 0.06666667f, 1.0f);
	colors[41] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[42] = ImVec4(0.15f, 0.15f, 0.15f, 1.0f);
	colors[43] = ImVec4(0.12156863f, 0.12156863f, 0.1254902f, 1.0f);
	colors[44] = ImVec4(1.0f, 0.0f, 1.0f, 1.0f);
	colors[45] = ImVec4(0.0f, 1.0f, 1.0f, 1.0f);
	colors[46] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);
	colors[47] = ImVec4(0.0f, 0.0f, 0.0f, 1.0f);
	colors[48] = ImVec4(1.0f, 0.0f, 1.0f, 1.0f);
	colors[49] = ImVec4(0.2f, 0.22f, 0.23f, 1.0f);
	colors[50] = ImVec4(0.33f, 0.67f, 0.86f, 1.0f);
	colors[51] = ImVec4(0.24f, 0.24f, 0.24f, 1.0f);
	colors[52] = ImVec4(1.0f, 0.0f, 1.0f, 1.0f);
	colors[53] = ImVec4(0.0f, 1.0f, 1.0f, 1.0f);
	colors[54] = ImVec4(0.1882353f, 0.1882353f, 0.20392157f, 1.0f);

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
	style.FrameBorderSize = 1.0f;
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

struct ExampleAppLog
{
    ImGuiTextBuffer     Buf;
    ImGuiTextFilter     Filter;
    ImVector<int>       LineOffsets; // Index to lines offset. We maintain this with AddLog() calls.
    bool                AutoScroll;  // Keep scrolling if already at the bottom.

    ExampleAppLog()
    {
        AutoScroll = true;
        Clear();
    }

    void    Clear()
    {
        Buf.clear();
        LineOffsets.clear();
        LineOffsets.push_back(0);
    }

    void    AddLog(const char* fmt, ...) IM_FMTARGS(2)
    {
        int old_size = Buf.size();
        va_list args;
        va_start(args, fmt);
        Buf.appendfv(fmt, args);
        va_end(args);
        for (int new_size = Buf.size(); old_size < new_size; old_size++)
            if (Buf[old_size] == '\n')
                LineOffsets.push_back(old_size + 1);
    }

    void    Draw(const char* title, bool* p_open = NULL)
    {
        if (!ImGui::Begin(title, p_open))
        {
            ImGui::End();
            return;
        }

        // Options menu
        if (ImGui::BeginPopup("Options"))
        {
            ImGui::Checkbox("Auto-scroll", &AutoScroll);
            ImGui::EndPopup();
        }

        // Main window
        if (ImGui::Button("Options"))
            ImGui::OpenPopup("Options");
        ImGui::SameLine();
        bool clear = ImGui::Button("Clear");
        ImGui::SameLine();
        bool copy = ImGui::Button("Copy");
        ImGui::SameLine();
        Filter.Draw("Filter", -100.0f);

        ImGui::Separator();

        if (ImGui::BeginChild("scrolling", ImVec2(0, 0), false, ImGuiWindowFlags_HorizontalScrollbar))
        {
            if (clear)
                Clear();
            if (copy)
                ImGui::LogToClipboard();

            ImGui::PushStyleVar(ImGuiStyleVar_ItemSpacing, ImVec2(0, 0));
            const char* buf = Buf.begin();
            const char* buf_end = Buf.end();
            if (Filter.IsActive())
            {
                // In this example we don't use the clipper when Filter is enabled.
                // This is because we don't have random access to the result of our filter.
                // A real application processing logs with ten of thousands of entries may want to store the result of
                // search/filter.. especially if the filtering function is not trivial (e.g. reg-exp).
                for (int line_no = 0; line_no < LineOffsets.Size; line_no++)
                {
                    const char* line_start = buf + LineOffsets[line_no];
                    const char* line_end = (line_no + 1 < LineOffsets.Size) ? (buf + LineOffsets[line_no + 1] - 1) : buf_end;
                    if (Filter.PassFilter(line_start, line_end))
                        ImGui::TextUnformatted(line_start, line_end);
                }
            }
            else
            {
                // The simplest and easy way to display the entire buffer:
                //   ImGui::TextUnformatted(buf_begin, buf_end);
                // And it'll just work. TextUnformatted() has specialization for large blob of text and will fast-forward
                // to skip non-visible lines. Here we instead demonstrate using the clipper to only process lines that are
                // within the visible area.
                // If you have tens of thousands of items and their processing cost is non-negligible, coarse clipping them
                // on your side is recommended. Using ImGuiListClipper requires
                // - A) random access into your data
                // - B) items all being the  same height,
                // both of which we can handle since we have an array pointing to the beginning of each line of text.
                // When using the filter (in the block of code above) we don't have random access into the data to display
                // anymore, which is why we don't use the clipper. Storing or skimming through the search result would make
                // it possible (and would be recommended if you want to search through tens of thousands of entries).
                ImGuiListClipper clipper;
                clipper.Begin(LineOffsets.Size);
                while (clipper.Step())
                {
                    for (int line_no = clipper.DisplayStart; line_no < clipper.DisplayEnd; line_no++)
                    {
                        const char* line_start = buf + LineOffsets[line_no];
                        const char* line_end = (line_no + 1 < LineOffsets.Size) ? (buf + LineOffsets[line_no + 1] - 1) : buf_end;
                        ImGui::TextUnformatted(line_start, line_end);
                    }
                }
                clipper.End();
            }
            ImGui::PopStyleVar();

            // Keep up at the bottom of the scroll region if we were already at the bottom at the beginning of the frame.
            // Using a scrollbar or mouse-wheel will take away from the bottom edge.
            if (AutoScroll && ImGui::GetScrollY() >= ImGui::GetScrollMaxY())
                ImGui::SetScrollHereY(1.0f);
        }
        ImGui::EndChild();
        ImGui::End();
    }
};

static ExampleAppLog logs;