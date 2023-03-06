#pragma once
// passes necessary calls through to the original binkw32.dll, now named binkw32_o.dll.
#pragma comment(linker, "/export:_BinkWait@4=binkw32_o._BinkWait@4")
#pragma comment(linker, "/export:_BinkDoFrame@4=binkw32_o._BinkDoFrame@4")
#pragma comment(linker, "/export:_BinkCopyToBuffer@28=binkw32_o._BinkCopyToBuffer@28")
#pragma comment(linker, "/export:_BinkNextFrame@4=binkw32_o._BinkNextFrame@4")
#pragma comment(linker, "/export:_BinkOpen@8=binkw32_o._BinkOpen@8")
#pragma comment(linker, "/export:_BinkGetError@0=binkw32_o._BinkGetError@0")
#pragma comment(linker, "/export:_BinkOpenDirectSound@4=binkw32_o._BinkOpenDirectSound@4")
#pragma comment(linker, "/export:_BinkSetSoundSystem@8=binkw32_o._BinkSetSoundSystem@8")
#pragma comment(linker, "/export:_BinkClose@4=binkw32_o._BinkClose@4")