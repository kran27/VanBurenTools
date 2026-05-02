using System;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using Veldrid.Sdl2;

class Sdl2WindowExtensions
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate IntPtr SDL_RWFromConstMem_t(byte* mem, int size);
    private static SDL_RWFromConstMem_t s_RWFromConstMem = Sdl2Native.LoadFunction<SDL_RWFromConstMem_t>("SDL_RWFromConstMem");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SDL_LoadBMP_RW_t(IntPtr src, int freesrc);
    private static SDL_LoadBMP_RW_t s_LoadBMP_RW = Sdl2Native.LoadFunction<SDL_LoadBMP_RW_t>("SDL_LoadBMP_RW");
    private static IntPtr SDL_LoadBMP_RW(IntPtr src, int freesrc) => s_LoadBMP_RW(src, freesrc);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SDL_SetWindowIcon_t(IntPtr window, IntPtr surface);
    private static SDL_SetWindowIcon_t s_SetWindowIcon = Sdl2Native.LoadFunction<SDL_SetWindowIcon_t>("SDL_SetWindowIcon");
    private static void SDL_SetWindowIcon(IntPtr window, IntPtr surface) => s_SetWindowIcon(window, surface);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SDL_FreeSurface_t(IntPtr surface);
    private static SDL_FreeSurface_t s_FreeSurface = Sdl2Native.LoadFunction<SDL_FreeSurface_t>("SDL_FreeSurface");
    private static void SDL_FreeSurface(IntPtr surface) => s_FreeSurface(surface);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SDL_CreateSystemCursor_t(int id);
    private static SDL_CreateSystemCursor_t s_CreateSystemCursor = Sdl2Native.LoadFunction<SDL_CreateSystemCursor_t>("SDL_CreateSystemCursor");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SDL_SetCursor_t(IntPtr cursor);
    private static SDL_SetCursor_t s_SetCursor = Sdl2Native.LoadFunction<SDL_SetCursor_t>("SDL_SetCursor");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SDL_ShowCursor_t(int toggle);
    private static SDL_ShowCursor_t s_ShowCursor = Sdl2Native.LoadFunction<SDL_ShowCursor_t>("SDL_ShowCursor");

    private const int SDL_SYSTEM_CURSOR_ARROW = 0;
    private const int SDL_SYSTEM_CURSOR_IBEAM = 1;
    private const int SDL_SYSTEM_CURSOR_SIZENWSE = 5;
    private const int SDL_SYSTEM_CURSOR_SIZENESW = 6;
    private const int SDL_SYSTEM_CURSOR_SIZEWE = 7;
    private const int SDL_SYSTEM_CURSOR_SIZENS = 8;
    private const int SDL_SYSTEM_CURSOR_SIZEALL = 9;
    private const int SDL_SYSTEM_CURSOR_NO = 10;
    private const int SDL_SYSTEM_CURSOR_HAND = 11;

    private static readonly IntPtr[] s_cursorCache = new IntPtr[(int)ImGuiMouseCursor.COUNT];
    private static ImGuiMouseCursor s_lastCursor = (ImGuiMouseCursor)(-2);

    private static IntPtr GetOrCreateCursor(ImGuiMouseCursor cursor)
    {
        var idx = (int)cursor;
        if (idx < 0 || idx >= s_cursorCache.Length) return IntPtr.Zero;
        if (s_cursorCache[idx] != IntPtr.Zero) return s_cursorCache[idx];

        var sdlId = cursor switch
        {
            ImGuiMouseCursor.Arrow => SDL_SYSTEM_CURSOR_ARROW,
            ImGuiMouseCursor.TextInput => SDL_SYSTEM_CURSOR_IBEAM,
            ImGuiMouseCursor.ResizeAll => SDL_SYSTEM_CURSOR_SIZEALL,
            ImGuiMouseCursor.ResizeNS => SDL_SYSTEM_CURSOR_SIZENS,
            ImGuiMouseCursor.ResizeEW => SDL_SYSTEM_CURSOR_SIZEWE,
            ImGuiMouseCursor.ResizeNESW => SDL_SYSTEM_CURSOR_SIZENESW,
            ImGuiMouseCursor.ResizeNWSE => SDL_SYSTEM_CURSOR_SIZENWSE,
            ImGuiMouseCursor.Hand => SDL_SYSTEM_CURSOR_HAND,
            ImGuiMouseCursor.NotAllowed => SDL_SYSTEM_CURSOR_NO,
            _ => SDL_SYSTEM_CURSOR_ARROW,
        };
        s_cursorCache[idx] = s_CreateSystemCursor(sdlId);
        return s_cursorCache[idx];
    }

    internal static void UpdateMouseCursor()
    {
        var io = ImGui.GetIO();
        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0) return;

        var requested = ImGui.GetMouseCursor();
        if (requested == s_lastCursor) return;
        s_lastCursor = requested;

        if (io.MouseDrawCursor || requested == ImGuiMouseCursor.None)
        {
            s_ShowCursor(0);
        }
        else
        {
            var handle = GetOrCreateCursor(requested);
            if (handle != IntPtr.Zero) s_SetCursor(handle);
            s_ShowCursor(1);
        }
    }

    private static unsafe IntPtr SDL_RWFromConstMem(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            return s_RWFromConstMem(dataPtr, data.Length);
        }
    }

    private static IntPtr SDL_LoadBMPFromMemory(byte[] data)
    {
        var rwOps = SDL_RWFromConstMem(data);
        return SDL_LoadBMP_RW(rwOps, 1); // Pass 1 to free the RWops after use
    }

    internal static unsafe void SetWindowIcon(Sdl2Window sdlWindow, byte[] bmpData)
    {
        var handle = sdlWindow.SdlWindowHandle;
        if (bmpData == null || bmpData.Length == 0)
        {
            throw new Exception("Application icon data is null or empty.");
        }

        var icon = SDL_LoadBMPFromMemory(bmpData);
        if (icon != IntPtr.Zero)
        {
            SDL_SetWindowIcon(handle, icon);
            SDL_FreeSurface(icon);
            return;
        }

        var error = Sdl2Native.SDL_GetError();
        if (error == null)
        {
            return;
        }

        var chars = 0;
        while (error[chars] != 0)
        {
            chars++;
        }

        throw new Exception($"Failed to load application icon: {Encoding.UTF8.GetString(error, chars)}");
    }
}
