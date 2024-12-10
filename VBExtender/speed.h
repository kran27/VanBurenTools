#pragma once
#include "detours.h"
#include <Windows.h>

template <class DataType> class SpeedHack {
  DataType time_offset;
  DataType time_last_update;

  double speed_;

public:
  SpeedHack(DataType currentRealTime, double initialSpeed) {
    time_offset = currentRealTime;
    time_last_update = currentRealTime;

    speed_ = initialSpeed;
  }

  // TODO: put lock around for thread safety
  void setSpeed(DataType currentRealTime, double speed) {
    time_offset = getCurrentTime(currentRealTime);
    time_last_update = currentRealTime;

    speed_ = speed;
  }

  // TODO: put lock around for thread safety
  DataType getCurrentTime(DataType currentRealTime) {
    DataType difference = currentRealTime - time_last_update;

    return (DataType)(speed_ * difference) + time_offset;
  }
};

// function signature typedefs
typedef DWORD(WINAPI *GetTickCountType)(void);

typedef BOOL(WINAPI *QueryPerformanceCounterType)(
    LARGE_INTEGER *lpPerformanceCount);

// globals
GetTickCountType g_GetTickCountOriginal;
GetTickCountType
    g_TimeGetTimeOriginal; // Same function signature as GetTickCount

QueryPerformanceCounterType g_QueryPerformanceCounterOriginal;

const double kInitialSpeed = 1.0; // initial speed hack speed

//                                  (initialTime,      initialSpeed)
SpeedHack<DWORD> g_speedHack(GetTickCount(), kInitialSpeed);
SpeedHack<LONGLONG>
    g_speedHackLL(0, kInitialSpeed); // Gets set properly in DllMain

// function prototypes

DWORD WINAPI GetTickCountHacked(void);
BOOL WINAPI QueryPerformanceCounterHacked(LARGE_INTEGER *lpPerformanceCount);
DWORD WINAPI KeysThread(LPVOID lpThreadParameter);

void setAllToSpeed(double speed) {
  g_speedHack.setSpeed(g_GetTickCountOriginal(), speed);

  LARGE_INTEGER performanceCounter;
  g_QueryPerformanceCounterOriginal(&performanceCounter);

  g_speedHackLL.setSpeed(performanceCounter.QuadPart, speed);
}

DWORD WINAPI GetTickCountHacked(void) {
  return g_speedHack.getCurrentTime(g_GetTickCountOriginal());
}

BOOL WINAPI QueryPerformanceCounterHacked(LARGE_INTEGER *lpPerformanceCount) {
  LARGE_INTEGER performanceCounter;

  BOOL result = g_QueryPerformanceCounterOriginal(&performanceCounter);

  lpPerformanceCount->QuadPart =
      g_speedHackLL.getCurrentTime(performanceCounter.QuadPart);

  return result;
}