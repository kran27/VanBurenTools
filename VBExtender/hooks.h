#pragma once
#include <iostream>

#include "speed.h"
#include "detours.h"
#include "functions.h"
#include "globals.h"
#include "imguiextensions.h"

typedef void (WINAPI* OutputDebugStringAFunc)(LPCSTR lpOutputString);
OutputDebugStringAFunc OriginalOutputDebugStringA;

bool initReady = false;

inline void WINAPI HookedOutputDebugStringA(const LPCSTR lpOutputString)
{
	std::string debugString(lpOutputString);

	// Ensure the string ends with a newline
	if (!debugString.empty() && debugString.back() != '\n') {
		debugString += '\n';
	}

	// Only initialize after Client is connected
	if (!initReady && debugString == "DIALOGUEINTERFACE INSTANTIATED\n - This debug line is to double check that it's not being created more than once.\n") {
		initReady = true;
	}
	if (debugString == "Script_Small::Debugger() Attempt to set SRANGE on non-array LocalVar.\n")
	{
		return; // Ignore this specific debug message as it is spammed in the console
	}

	logs.AddLog(debugString.c_str());

	OriginalOutputDebugStringA(debugString.c_str());
}

void InitHooks()
{
	HMODULE kernel32 = GetModuleHandleA("kernel32.dll");
	HMODULE winmm = GetModuleHandleA("winmm.dll");

	OriginalOutputDebugStringA = reinterpret_cast<OutputDebugStringAFunc>(GetProcAddress(kernel32, "OutputDebugStringA"));
	g_GetTickCountOriginal = (GetTickCountType)GetProcAddress(kernel32, "GetTickCount");
	g_TimeGetTimeOriginal = (GetTickCountType)GetProcAddress(winmm, "timeGetTime");
	g_QueryPerformanceCounterOriginal = (QueryPerformanceCounterType)GetProcAddress(kernel32, "QueryPerformanceCounter");

	// Setup the speed hack object for the Performance Counter
	LARGE_INTEGER performanceCounter;
	g_QueryPerformanceCounterOriginal(&performanceCounter);

	g_speedHackLL = SpeedHack<LONGLONG>(performanceCounter.QuadPart, kInitialSpeed);

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(reinterpret_cast<PVOID*>(&OriginalOutputDebugStringA), HookedOutputDebugStringA);
	DetourAttach(reinterpret_cast<PVOID*>(&g_GetTickCountOriginal), GetTickCountHacked);
	DetourAttach(reinterpret_cast<PVOID*>(&g_TimeGetTimeOriginal), GetTickCountHacked);
	DetourAttach(reinterpret_cast<PVOID*>(&g_QueryPerformanceCounterOriginal), QueryPerformanceCounterHacked);
	DetourTransactionCommit();
}