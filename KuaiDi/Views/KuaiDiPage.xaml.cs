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
    public sealed partial class KuaiDiPage : Page
    {
        public KuaiDiPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            //await Class.KD_Class.Clear();
            var kd_list =await Class.KD_Class.Get();
            if (kd_list.Count != kdListView.Items.Count)
            {
                kdListView.Items.Clear();
                foreach (var kd in kd_list)
                {
                    var kd_net = new Class.KD_Class.KD_Model_Net(kd);
                    await kd_net.Load();
                    kdListView.Items.Add(kd_net);
                }
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void AddkdBtnClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new Add_KD_Dialog();
            if(await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                LoadData();
            }
        }

        private void kdListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListView;
            if (list.SelectedIndex != -1)
            {
                var kd = list.SelectedItem as Class.KD_Class.KD_Model_Net;
                if(kd.MoreListVisibility== "Collapsed")
                {
                    kd.MoreListVisibility = "Visible";
                }else
                {
                    kd.MoreListVisibility = "Collapsed";
                }
                kd.RaisePropertyChanged("MoreListVisibility");
                list.SelectedIndex = -1;
            }
        }

        private async void DelKDBtnCLicked(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog("是否删除该快递跟踪?");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand() { Id = 0, Label = "删除" });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand() { Id = 1, Label = "取消" });
            dialog.DefaultCommandIndex = 0;
            if((int)(await dialog.ShowAsync()).Id == 0)
            {
                var btn = sender as AppBarButton;
                var kd = btn.DataContext as Class.KD_Class.KD_Model_Net;
                await Class.KD_Class.Remove(kd);
                for (int i = 0; i < kdListView.Items.Count; i++)
                {
                    if ((kdListView.Items[i] as Class.KD_Class.KD_Model_Net).num == kd.num)
                    {
                        kdListView.Items.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
