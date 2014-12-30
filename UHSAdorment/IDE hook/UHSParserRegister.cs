using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace UHSAdorment.IDE_hook
{
    class UHSParserRegister
    {
        void registerParser(EnvDTE.DTE dte)
        {
            ServiceProvider serviceProvider = new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

            RegistryKey userSettings = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings);

            RegistryKey filexts = userSettings.OpenSubKey("FileExtensionMapping", true);

            RegistryKey UhsKey = filexts.OpenSubKey("uhs", false);
            if (UhsKey == null)
            {
                UhsKey = filexts.CreateSubKey("uhs");
                UhsKey.SetValue(null, "{8B382828-6202-11D1-8870-0000F87579D2}");
                UhsKey.SetValue("LogViewID", "{B2F072B0-ABC1-11D0-9D62-00C04FD9DFD9}");
                UhsKey.Close();
                filexts.Close();
            }
            else
            {
                UhsKey.Close();
                filexts.Close();
            }
        }
    }
}
