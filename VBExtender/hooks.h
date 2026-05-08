#pragma once
#include <iostream>
#include <algorithm>
#include <cstring>
#include <string>

#include "speed.h"
#include "detours.h"
#include "functions.h"
#include "globals.h"
#include "imguiextensions.h"

typedef void (WINAPI* OutputDebugStringAFunc)(LPCSTR lpOutputString);
OutputDebugStringAFunc OriginalOutputDebugStringA;

bool initReady = false;

// ----- b3d transform-source diagnostic hooks -----
//
// Goal: when a b3d whose name matches one of the targets below is loaded,
// log the bytes the engine ends up using for its per-mesh world transform.
// We can't infer the rotation source from the file alone (verified by
// scanning all 3915 b3d files: zero have 0x0D NodeTransform / zero have
// 0x17 records / scene graphs are flat) — the engine must be deriving the
// rotation at runtime from a signal we haven't found, so we capture state
// directly.
//
// Hook 1: ResourceSystem::MemStreamOpen (engine 0x49beb0). When the
// filename matches a target, set a flag so the next outer mesh-prep call
// is logged.
//
// Hook 2: sub_529490 (engine 0x529490) — the function that takes a parsed
// b3d wrapper, applies the LEFT_HANDED conjugate flip to every bone, walks
// the scene graph via RecurseChildNode, and returns a fully-prepared
// Gfx_G3D_Mesh. Log the input wrapper's first byte (LEFT_HANDED flag) and
// the produced mesh's transform fields.

static const char* kB3dLogTargets[] = {
    "ventrance", "console", "glass", "ds_mesa",
};

static bool B3dLogMatches(const char* filename) {
    if (!filename) return false;
    std::string s = filename;
    std::transform(s.begin(), s.end(), s.begin(), [](unsigned char c){ return (char)std::tolower(c); });
    // Drop the ".b3d" requirement: the engine may pass paths or extensionless
    // names. Match purely on the target tokens.
    for (auto* t : kB3dLogTargets)
        if (s.find(t) != std::string::npos) return true;
    return false;
}

// Diagnostic: log a sample of every MemStreamOpen (first N + every Mth) so
// we can confirm the hook is actually firing and see what filename strings
// the engine actually produces. Cap so we don't flood the log.
static volatile int g_b3dDiagOpenCount = 0;

// __thiscall on x86 = `this` in ECX, rest on stack. Match with __fastcall
// + a dummy EDX parameter (which __thiscall doesn't use but __fastcall
// does — declaring it as void* keeps the stack aligned with the engine's
// expected args).
typedef int* (__fastcall *MemStreamOpenT)(void* this_, void* edx_unused, const char* filename, char a3);
MemStreamOpenT g_MemStreamOpenOriginal = nullptr;

typedef void* (__fastcall *Sub529490T)(void* this_, void* edx_unused, int a2, int a3, char* a4, void* a5, float a6);
Sub529490T g_Sub529490Original = nullptr;

typedef void (__fastcall *RecurseChildT)(void* this_, void* edx_unused, int a2, float* a3, int a4, float a5, int a6, char* a7, void* a8);
RecurseChildT g_RecurseChildOriginal = nullptr;

static char g_b3dLastTargetName[260] = {0};
static volatile bool g_b3dExpectNextMesh = false;
// Active depth counter for our target's sub_529490 call. When > 0, every
// RecurseChildNode invocation logs a3 (the per-mesh transform output).
static volatile int g_b3dInsideTargetDepth = 0;
static int g_b3dRecurseCallCounter = 0;

inline int* __fastcall HookedMemStreamOpen(void* this_, void* edx, const char* filename, char a3) {
    int n = ++g_b3dDiagOpenCount;
    // Log first 30 opens + every 200th after, so we always see proof the hook
    // is alive plus a sample of filename shapes. Filter-matched opens always
    // log regardless.
    bool diag = (n <= 30) || (n % 200 == 0);
    bool match = filename && B3dLogMatches(filename);
    if (diag || match) {
        logs.AddLog("[b3d] open #%d %s%s\n",
            n,
            match ? "[MATCH] " : "",
            filename ? filename : "<null>");
    }
    if (match) {
        std::strncpy(g_b3dLastTargetName, filename, sizeof(g_b3dLastTargetName) - 1);
        g_b3dLastTargetName[sizeof(g_b3dLastTargetName) - 1] = 0;
        g_b3dExpectNextMesh = true;
    }
    return g_MemStreamOpenOriginal(this_, edx, filename, a3);
}

inline void* __fastcall HookedSub529490(void* this_, void* edx, int a2, int a3, char* a4, void* a5, float a6) {
    bool wasTarget = g_b3dExpectNextMesh;
    char savedName[260];
    if (wasTarget) {
        std::memcpy(savedName, g_b3dLastTargetName, sizeof(savedName));
        g_b3dExpectNextMesh = false;
    }

    if (wasTarget) {
        unsigned char flagByte = this_ ? *(unsigned char*)this_ : 0xFF;
        void* innerModel = this_ ? *((void**)this_ + 1) : nullptr;
        logs.AddLog("[b3d] PRE  %s wrapper=%p flagByte=%u inner=%p scale=%f\n",
            savedName, this_, (unsigned)flagByte, innerModel, a6);
        // Turn on per-RecurseChildNode logging for the duration of this call
        // so we capture the per-mesh transforms the engine actually writes.
        g_b3dInsideTargetDepth++;
        g_b3dRecurseCallCounter = 0;
    }

    void* result = g_Sub529490Original(this_, edx, a2, a3, a4, a5, a6);

    if (wasTarget) {
        unsigned char flagByte = this_ ? *(unsigned char*)this_ : 0xFF;
        logs.AddLog("[b3d] POST %s wrapper=%p flagByte(post)=%u result=%p recurseCalls=%d\n",
            savedName, this_, (unsigned)flagByte, result, g_b3dRecurseCallCounter);
        g_b3dInsideTargetDepth--;
    }
    return result;
}

// RecurseChildNode hook. When we're inside a target model's sub_529490 call
// (g_b3dInsideTargetDepth > 0), capture the per-mesh transform array `a3`
// before/after the engine fills it in. The engine writes a quaternion via
// sub_49D1B0(a3, &quat) and translation at a3[8..10] / a3[23..25]. The
// quaternion lands at one of a3's earlier offsets — dumping a3[0..31] both
// pre and post lets us identify exactly where it ends up without further
// reverse-engineering.
inline void __fastcall HookedRecurseChild(void* this_, void* edx, int a2, float* a3, int a4, float a5, int a6, char* a7, void* a8) {
    bool log = (g_b3dInsideTargetDepth > 0);
    int callIdx = log ? ++g_b3dRecurseCallCounter : 0;
    float pre[32];
    if (log && a3) {
        std::memcpy(pre, a3, sizeof(pre));
    }

    g_RecurseChildOriginal(this_, edx, a2, a3, a4, a5, a6, a7, a8);

    if (log && a3) {
        unsigned char wrapperByte0 = this_ ? *(unsigned char*)this_ : 0xFF;
        logs.AddLog("[b3d] Recurse#%d a3=%p this=%p wrapByte=%u node=%p scale=%f\n",
            callIdx, a3, this_, (unsigned)wrapperByte0, (void*)a2, a5);
        for (int i = 0; i < 32; i += 4) {
            logs.AddLog("    pre [%2d..%2d]  % .4f  % .4f  % .4f  % .4f\n",
                i, i+3, pre[i], pre[i+1], pre[i+2], pre[i+3]);
        }
        for (int i = 0; i < 32; i += 4) {
            logs.AddLog("    post[%2d..%2d]  % .4f  % .4f  % .4f  % .4f\n",
                i, i+3, a3[i], a3[i+1], a3[i+2], a3[i+3]);
        }
    }
}

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

	// b3d transform diagnostic hooks — engine offsets relative to F3 base
	// (image base 0x400000, default for F3.exe).
	g_MemStreamOpenOriginal = reinterpret_cast<MemStreamOpenT>(F3 + 0x9beb0);  // ResourceSystem::MemStreamOpen
	g_Sub529490Original     = reinterpret_cast<Sub529490T>    (F3 + 0x129490); // sub_529490 (mesh prep + scene walk)
	g_RecurseChildOriginal  = reinterpret_cast<RecurseChildT> (F3 + 0x127fd0); // RecurseChildNode (per-mesh transform writer)

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(reinterpret_cast<PVOID*>(&OriginalOutputDebugStringA), HookedOutputDebugStringA);
	DetourAttach(reinterpret_cast<PVOID*>(&g_GetTickCountOriginal), GetTickCountHacked);
	DetourAttach(reinterpret_cast<PVOID*>(&g_TimeGetTimeOriginal), GetTickCountHacked);
	DetourAttach(reinterpret_cast<PVOID*>(&g_QueryPerformanceCounterOriginal), QueryPerformanceCounterHacked);
	DetourAttach(reinterpret_cast<PVOID*>(&g_MemStreamOpenOriginal), HookedMemStreamOpen);
	DetourAttach(reinterpret_cast<PVOID*>(&g_Sub529490Original),     HookedSub529490);
	DetourAttach(reinterpret_cast<PVOID*>(&g_RecurseChildOriginal),  HookedRecurseChild);
	LONG b3dHookErr = DetourTransactionCommit();

	logs.AddLog("[b3d] InitHooks done. F3=%p MemStreamOpen=%p sub_529490=%p RecurseChild=%p commit=%ld\n",
		(void*)F3,
		(void*)g_MemStreamOpenOriginal,
		(void*)g_Sub529490Original,
		(void*)g_RecurseChildOriginal,
		(long)b3dHookErr);
}