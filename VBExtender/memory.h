#pragma once
#include <vector>
#include <cstdint>

template<typename T>
T Read(uintptr_t address) {
	if (IsBadReadPtr((void*)address, sizeof(T))) return 0;
	return *(T*)address;
}

template<typename T>
void Write(uintptr_t address, T data) {
	if (IsBadReadPtr((void*)address, sizeof(T))) return;
	if (IsBadWritePtr((LPVOID)address, sizeof(T))) return;
	*(T*)address = data;
}

template<std::size_t N>
void WriteStr(uintptr_t address, char const(&data)[N]) {
    if (IsBadReadPtr((void*)address, N)) return;
    if (IsBadWritePtr((LPVOID)address, N)) return;
    std::memcpy((void*)address, data, N);
}


uintptr_t GetAddr(uintptr_t base, const std::vector<uintptr_t> offsets) {
	for (unsigned int offset : offsets)
	{
		base = Read<uintptr_t>(base);
		if (!base) return 0;
		base += offset;
		if (!base) return 0;
	}
	if (IsBadReadPtr((void*)base, sizeof(int))) return (uintptr_t)nullptr;
	return base;
}

template<typename T>
T ReadPtr(uintptr_t base, const std::vector<uintptr_t> offsets) {
	return Read<T>(GetAddr(base, offsets));
}

template<typename T>
void WritePtr(uintptr_t base, const std::vector<uintptr_t> offsets, T data) {;
	Write<T>(GetAddr(base, offsets), data);
}