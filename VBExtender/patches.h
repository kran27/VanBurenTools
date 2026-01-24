// Helper function to apply a byte patch at a specific offset in the game executable. from a separate codebase of mine.
inline bool apply_byte_patch(const uintptr_t offset, const std::vector<uint8_t>& data)
{
    DWORD old_protect;
    if (auto addr = reinterpret_cast<LPVOID>(F3 + offset); VirtualProtect(addr, data.size(), PAGE_EXECUTE_READWRITE, &old_protect))
    {
        Write(reinterpret_cast<uintptr_t>(addr), data);
        VirtualProtect(addr, data.size(), old_protect, &old_protect);
        return true;
    }
    return false;
}

void PatchInstaCrash()
{
    // Patch out the insta crash when multiple instances of the game are running
    // this is done by patching jnz to jmp so errors aren't thrown when the socket is already in use
    apply_byte_patch(0x1B8F46, {0xEB});
    apply_byte_patch(0x1B8DE7, {0xEB});
}

void ForceScriptDebug()
{
    // Forces all scripts to be run with debug hooks enabled.
    // (patches out conditional checks so debug hooks are used regardless of parameters)
    apply_byte_patch(0x1A89ED, {0x90,0x90,0x90,0x90,0x90,0x90});
    apply_byte_patch(0x1A89F9, {0x90,0x90});
    apply_byte_patch(0x1A8A01, {0x90,0x90});
}

void ApplyPatches()
{
    PatchInstaCrash();
    ForceScriptDebug();
}