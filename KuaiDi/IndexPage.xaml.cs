using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
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

namespace KuaiDi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page
    {
        public IndexPage()
        {
            this.InitializeComponent();
            BackGround_reg();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                mainFrame.Navigate(typeof(Views.KuaiDiPage));
            }
        }

        private void splitViewOpenClicked(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private async void BackGround_reg()
        {
            try
            {
                var status = await BackgroundExecutionManager.RequestAccessAsync();
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == "KuaiDiBg")
                    {
                        return;
                    }
                }
                var builder = new BackgroundTaskBuilder();
                builder.Name = "KuaiDiBg";
                builder.TaskEntryPoint = "KuaiDiBg.KDBackgroundTask";
                builder.SetTrigger(new TimeTrigger(30, false));
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                var task = builder.Register();
            }
            catch (Exception)
            {

            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var list = sender as ListBox;
                switch (list.SelectedIndex)
                {
                    case 0:
                        mainFrame.Navigate(typeof(Views.KuaiDiPage));
                        break;
                    case 1:
                        mainFrame.Navigate(typeof(Views.SettingPage));
                        break;
                    case 2:
                        mainFrame.Navigate(typeof(Views.AboutPage));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
