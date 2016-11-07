using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace KuaiDi
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
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
            int num = new Random().Next(0, ColorList.Count);
            var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSetting.Values.ContainsKey("Theme"))
            {
                switch ((int)localSetting.Values["Theme"])
                {
                    case 0:
                        Class.Theme_Class.ChangeThemeColor();
                        break;
                    case 1:
                        Class.Theme_Class.ChangeThemeColor(ColorList[num]);
                        break;
                    case 2:
                        Class.Theme_Class.ChangeThemeColor(ColorList[(int)localSetting.Values["CustomTheme"]]);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Class.Theme_Class.ChangeThemeColor();
                
            }
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(IndexPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
