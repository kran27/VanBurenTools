using System;
using System.Runtime.InteropServices;
using System.Text;
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
