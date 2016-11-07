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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace KuaiDi.Views
{
    public sealed partial class Add_KD_Dialog : ContentDialog
    {
        public Add_KD_Dialog()
        {
            this.InitializeComponent();
            foreach (var item in Class.KD_Class.comDir)
            {
                kd_com.Items.Add(item.Key);
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var _deferral = args.GetDeferral();
            if (kd_name.Text==""||kd_num.Text == "" || kd_com.SelectedIndex == -1)
            {
                //var asd = args.GetDeferral();
                await new Windows.UI.Popups.MessageDialog("请将快递信息填写完整").ShowAsync();
                args.Cancel = true;
            }else
            {
                var kd = new Class.KD_Class.KD_Model(kd_name.Text, kd_num.Text, Class.KD_Class.comDir.Values.ToArray()[kd_com.SelectedIndex]);
                if(await Class.KD_Class.CanAdd(kd))
                {
                    await Class.KD_Class.Add(kd);
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("不要重复添加该快递").ShowAsync();
                    args.Cancel = true;
                }
            }
            _deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
