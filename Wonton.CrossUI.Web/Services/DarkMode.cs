// using ElectronNET.API;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Runtime.InteropServices;
// using System.Threading.Tasks;
// using Windows.UI;
// using Windows.UI.ViewManagement;

// namespace Wonton.CrossUI.Web.Services
// {
//     public class DarkMode
//     {
//         public static bool InDarkMode { get; private set; } = false;

//         public static event Action<bool> OnDarkModeChanged;

//         static DarkMode()
//         {
//             if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//             { 
//                 var settings = new UISettings();
//                 settings.ColorValuesChanged += (set, o) =>
//                 {
//                     InDarkMode = IsDarkModeWin(set);
//                     OnDarkModeChanged?.Invoke(InDarkMode);
//                 };
//                 InDarkMode = IsDarkModeWin(settings);
//             }
//         }

//         private static bool IsDarkModeWin(UISettings settings)
//         {
//             var foreground = settings.GetColorValue(UIColorType.Foreground);
//             var background = settings.GetColorValue(UIColorType.Background);
                
//             Debug.WriteLine($"Foreground {foreground} Background {background}");
//             return background.ToString() == "#FF000000";
//         }
//     }
// }
