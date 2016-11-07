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
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KuaiDi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localData = ApplicationData.Current.LocalSettings;
        Dictionary<string, string> comDir = new Dictionary<string, string>();

        public MainPage()
        {
            this.InitializeComponent();
            BackGround_reg();
            comDir.Add("EMS快递", "ems");
            comDir.Add("申通快递", "shentong");
            comDir.Add("圆通快递", "yuantong");
            comDir.Add("中通快递", "zhongtong");
            comDir.Add("汇通快递", "huitong");
            comDir.Add("天天快递", "tiantian");
            comDir.Add("韵达快递", "yunda");
            comDir.Add("顺丰快递", "shunfeng");
            comDir.Add("宅急送快递", "zhaijisong");
            comDir.Add("中国邮政", "pingyou");
            foreach (var item in comDir)
            {
                KuaiDicombox.Items.Add(item.Key);
            }
            Load_data();
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

        private void Load_data()
        {
            if (localData.Values.ContainsKey("KuaiDiData"))
            {
                var obj = Windows.Data.Json.JsonObject.Parse((string)localData.Values["KuaiDiData"]);
                kd_name.Text = obj.GetNamedString("name");
                kd_num.Text = obj.GetNamedString("num");
                for (int i = 0; i < comDir.Count;i++)
                {
                    if (obj.GetNamedString("com") == comDir.Values.ToArray()[i])
                    {
                        KuaiDicombox.SelectedIndex = i;
                        break;
                    }
                }
            }else
            {
                kd_name.Text = "";
                kd_num.Text = "";
                KuaiDicombox.SelectedIndex = -1;
            }
        }

        private async void SaveKuaiDiData(object sender, RoutedEventArgs e)
        {
            if (kd_name.Text != "" && kd_num.Text != "" && KuaiDicombox.SelectedIndex > 0)
            {
                var obj = new Windows.Data.Json.JsonObject();
                obj.Add("name", Windows.Data.Json.JsonValue.CreateStringValue(kd_name.Text));
                obj.Add("num", Windows.Data.Json.JsonValue.CreateStringValue(kd_num.Text));
                obj.Add("com", Windows.Data.Json.JsonValue.CreateStringValue(comDir.Values.ToArray()[KuaiDicombox.SelectedIndex]));
                localData.Values["KuaiDiData"] = obj.ToString();
                await new Windows.UI.Popups.MessageDialog("已保存").ShowAsync();
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("请输入快递信息").ShowAsync();
            }
        }

        private void DelKuaiDiData(object sender, RoutedEventArgs e)
        {
            localData.DeleteContainer("KuaiDiData");
            Load_data();
        }
    }
}
