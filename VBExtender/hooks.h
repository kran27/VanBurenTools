#pragma once
#include <iostream>

#include "detours.h"
#include "functions.h"
#include "globals.h"
#include "imguiextensions.h"
#include "speed.h"

typedef void(WINAPI *OutputDebugStringAFunc)(LPCSTR lpOutputString);
OutputDebugStringAFunc OriginalOutputDebugStringA;

bool initReady = false;

inline void WINAPI HookedOutputDebugStringA(const LPCSTR lpOutputString) {
  // Only initialize after Client is connected (probably should do this smarter
  // than checking debug strings.)
  if (!initReady &&
      strcmp(lpOutputString,
             "DIALOGUEINTERFACE INSTANTIATED\n - This debug line is to double "
             "check that it's not being created more than once.") == 0)
    initReady = true;

  logs.AddLog(lpOutputString);

  OriginalOutputDebugStringA(lpOutputString);
}

void InitHooks() {
  HMODULE kernel32 = GetModuleHandleA("kernel32.dll");
  HMODULE winmm = GetModuleHandleA("winmm.dll");

  OriginalOutputDebugStringA = reinterpret_cast<OutputDebugStringAFunc>(
      GetProcAddress(kernel32, "OutputDebugStringA"));
  g_GetTickCountOriginal =
      (GetTickCountType)GetProcAddress(kernel32, "GetTickCount");
  g_TimeGetTimeOriginal =
      (GetTickCountType)GetProcAddress(winmm, "timeGetTime");
  g_QueryPerformanceCounterOriginal =
      (QueryPerformanceCounterType)GetProcAddress(kernel32,
                                                  "QueryPerformanceCounter");

  // Setup the speed hack object for the Performance Counter
  LARGE_INTEGER performanceCounter;
  g_QueryPerformanceCounterOriginal(&performanceCounter);

  g_speedHackLL =
      SpeedHack<LONGLONG>(performanceCounter.QuadPart, kInitialSpeed);

  DetourTransactionBegin();
  DetourUpdateThread(GetCurrentThread());
  DetourAttach(reinterpret_cast<PVOID *>(&OriginalOutputDebugStringA),
               HookedOutputDebugStringA);
  DetourAttach(reinterpret_cast<PVOID *>(&g_GetTickCountOriginal),
               GetTickCountHacked);
  DetourAttach(reinterpret_cast<PVOID *>(&g_TimeGetTimeOriginal),
               GetTickCountHacked);
  DetourAttach(reinterpret_cast<PVOID *>(&g_QueryPerformanceCounterOriginal),
               QueryPerformanceCounterHacked);
  DetourTransactionCommit();
}