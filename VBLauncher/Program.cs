using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using VBLauncher;
using VBLauncher.Properties;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Probe user.config: if a previous build wrote a config with an
        // unrecognised section (e.g. older app version, different
        // assembly identity), reading any property throws a
        // ConfigurationErrorsException and every Settings access
        // downstream — STFDir, EnabledMods, etc — crashes the app.
        // Delete the broken file so the next access reopens it fresh
        // with defaults.
        TryRecoverCorruptUserConfig();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }

    private static void TryRecoverCorruptUserConfig()
    {
        try
        {
            _ = Settings.Default.STFDir;
        }
        catch (ConfigurationErrorsException ex)
        {
            var path = (ex.InnerException as ConfigurationErrorsException)?.Filename
                       ?? ex.Filename;
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try { File.Delete(path); } catch { /* nothing more we can do */ }
            }
            Settings.Default.Reload();
        }
    }
}