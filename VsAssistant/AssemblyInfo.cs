using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("VsAssistant")]
[assembly: AssemblyDescription("Tray assistant with location timers for vsmuta.com")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Oleh Ivanitskyy")]
[assembly: AssemblyProduct("VsAssistant")]
[assembly: AssemblyCopyright("Copyright © Oleh Ivanitskyy 2024-2026")]

[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
