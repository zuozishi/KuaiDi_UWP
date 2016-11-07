using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KuaiDi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public bool TileSwitch
        {
            get
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSetting.Values.ContainsKey("TileSwitch"))
                {
                    return (bool)localSetting.Values["TileSwitch"];
                }
                else
                {
                    localSetting.Values["TileSwitch"] = true;
                    return true;
                }
            }
            set
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["TileSwitch"] = value;
            }
        }
        public bool NofSwitch
        {
            get
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSetting.Values.ContainsKey("NofSwitch"))
                {
                    return (bool)localSetting.Values["NofSwitch"];
                }
                else
                {
                    localSetting.Values["NofSwitch"] = true;
                    return true;
                }
            }
            set
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["NofSwitch"] = value;
            }
        }
        public ThemeSetting Theme
        {
            get
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSetting.Values.ContainsKey("Theme"))
                {
                    return (ThemeSetting)(int)localSetting.Values["Theme"];
                }
                else
                {
                    localSetting.Values["Theme"] = (int)ThemeSetting.Windows;
                    return ThemeSetting.Windows;
                }
            }
            set
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (value == ThemeSetting.Custom)
                {
                    Theme_Custom_Grid = Visibility.Visible;
                }
                else
                {
                    Theme_Custom_Grid = Visibility.Collapsed;
                }
                if (value == ThemeSetting.Windows)
                {
                    Class.Theme_Class.ChangeThemeColor();
                }
                localSetting.Values["Theme"] = (int)value;
            }
        }
        public enum ThemeSetting
        {
            Windows,Change,Custom
        }
        public bool? Theme_Windows
        {
            get
            {
                if (Theme == ThemeSetting.Windows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)
                {
                    Theme = ThemeSetting.Windows;
                }
            }
        }
        public bool? Theme_Change
        {
            get
            {
                if (Theme == ThemeSetting.Change)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)
                {
                    Theme = ThemeSetting.Change;
                }
            }
        }
        public bool? Theme_Custom
        {
            get
            {
                if (Theme == ThemeSetting.Custom)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)
                {
                    Theme = ThemeSetting.Custom;
                }
            }
        }
        public Visibility Theme_Custom_Grid
        {
            get
            {
                if (Theme_Custom == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {
                ColorGrid.Visibility = value;
            }
        }
        public int ColorGridSelected
        {
            get
            {
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSetting.Values.ContainsKey("CustomTheme"))
                {
                    return (int)localSetting.Values["CustomTheme"];
                }
                else
                {
                    localSetting.Values["CustomTheme"] = 0;
                    return 0;
                }
            }
            set
            {
                var ColorList = new List<Windows.UI.Color>();
                ColorList.Add(Windows.UI.Color.FromArgb(255, 238, 88, 88));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 231, 238, 88));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 163, 238, 88));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 108, 238, 88));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 88, 238, 108));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 88, 238, 170));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 88, 238, 101));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 88, 238, 0));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 226, 23, 60));
                ColorList.Add(Windows.UI.Color.FromArgb(255, 79, 168, 147));
                Class.Theme_Class.ChangeThemeColor(ColorList[value]);
                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSetting.Values["CustomTheme"] = value;
            }
        }
        public SettingPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:lockscreen"));
        }
    }
}
