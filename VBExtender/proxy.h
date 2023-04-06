#pragma once
#include "byte_arrays.h"
#include "MemoryModule/MemoryModule.h"

FARPROC p[9] = { nullptr };

// loads the .dll from an array and obtains addresses from there, instead of loading the .dll from disk.
void GetAddresses()
{
	const HMEMORYMODULE bink = MemoryLoadLibrary(binkw32, sizeof binkw32);

	p[0] = MemoryGetProcAddress(bink, "_BinkWait@4");
	p[1] = MemoryGetProcAddress(bink, "_BinkDoFrame@4");
	p[2] = MemoryGetProcAddress(bink, "_BinkCopyToBuffer@28");
	p[3] = MemoryGetProcAddress(bink, "_BinkNextFrame@4");
	p[4] = MemoryGetProcAddress(bink, "_BinkOpen@8");
	p[5] = MemoryGetProcAddress(bink, "_BinkGetError@0");
	p[6] = MemoryGetProcAddress(bink, "_BinkOpenDirectSound@4");
	p[7] = MemoryGetProcAddress(bink, "_BinkSetSoundSystem@8");
	p[8] = MemoryGetProcAddress(bink, "_BinkClose@4");
}

// _BinkWait@4
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkWait(int a1)
{
	__asm
	{
		jmp p[0 * 4];
	}
}

// _BinkDoFrame@4
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkDoFrame(int a1)
{
	__asm
	{
		jmp p[1 * 4];
	}
}

// _BinkCopyToBuffer@28
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkCopyToBuffer(int a1, int a2, int a3, int a4, int a5, int a6, int a7)
{
	__asm
	{
		jmp p[2 * 4];
	}
}

// _BinkNextFrame@4
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkNextFrame(int a1)
{
	__asm
	{
		jmp p[3 * 4];
	}
}

// _BinkOpen@8
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkOpen(int a1, int a2)
{
	__asm
	{
		jmp p[4 * 4];
	}
}

// _BinkGetError@0
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkGetError()
{
	__asm
	{
		jmp p[5 * 4];
	}
}

// _BinkOpenDirectSound@4
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkOpenDirectSound(int a1)
{
	__asm
	{
		jmp p[6 * 4];
	}
}

// _BinkSetSoundSystem@8
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkSetSoundSystem(int a1, int a2)
{
	__asm
	{
		jmp p[7 * 4];
	}
}

// _BinkClose@4
extern "C" __declspec(dllexport) __declspec(naked) void __stdcall BinkClose(int a1)
{
	__asm
	{
		jmp p[8 * 4];
	}
}