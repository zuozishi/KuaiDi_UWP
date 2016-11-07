using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI;
using Windows.UI.Notifications;

namespace KuaiDiBg
{
    public sealed class KDBackgroundTask : IBackgroundTask
    {
        Windows.Storage.ApplicationDataContainer localData = Windows.Storage.ApplicationData.Current.LocalSettings;
        async void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            if (localData.Values.ContainsKey("TileSwitch")&& (bool)localData.Values["TileSwitch"]==true)
            {
                await UpdateTile();
            }
            deferral.Complete();
        }

        private void ShowToast(KD_Model_Net kd)
        {
            var doc = new XmlDocument();
            doc.LoadXml(WebUtility.HtmlDecode(string.Format(ToastTemplateXml,kd.name,kd.com_cn,kd.newdata.context)), new XmlLoadSettings
            {
                ProhibitDtd = false,
                ValidateOnParse = false,
                ElementContentWhiteSpace = false,
                ResolveExternals = false
            });
            var toast=new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async Task UpdateTile()
        {
            try
            {
                var kd_list = await GetKD();
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueueForWide310x150(true);
                updater.EnableNotificationQueueForSquare150x150(true);
                updater.EnableNotificationQueueForSquare310x310(true);
                updater.EnableNotificationQueue(true);
                updater.Clear();
                foreach (var kd in kd_list)
                {
                    var kd_net = new KD_Model_Net(kd);
                    await kd_net.Load();
                    if (kd_net.message == "ok" && kd_net.ischeck == "0")
                    {
                        var tilexml = string.Format(TileTemplateXml, kd_net.name, kd_net.com_cn, kd_net.newdata.context, kd_net.newdata.time_s);
                        var doc = new Windows.Data.Xml.Dom.XmlDocument();
                        doc.LoadXml(WebUtility.HtmlDecode(tilexml), new XmlLoadSettings
                        {
                            ProhibitDtd = false,
                            ValidateOnParse = false,
                            ElementContentWhiteSpace = false,
                            ResolveExternals = false
                        });
                        updater.Update(new TileNotification(doc));
                        if(await isNew(kd_net))
                        {
                            ShowToast(kd_net);
                        }
                    }
                }
                //string tilexml = "<tile><binding template=\"TileMedium\"><text hint-style=\"subtitle\" id=\"0\">{0} {1} "+time_s+ "</text><text hint-wrap='true' id=\"1\">{2}</text></binding><binding template=\"TileWide\"><text hint-style=\"subtitle\">{0} {1} " + time_s + "</text><text hint-wrap='true'>{2}</text></binding><binding template=\"TileLarge\"><text hint-style=\"subtitle\">{0} {1} " + time_s + "</text><text hint-wrap='true'>{2}</text></binding></tile>";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private const string TileTemplateXml = @"
<tile branding='name'> 
  <visual version='2'>
    <binding template='TileMedium'>
      <text hint-wrap='true'>{0} {1} {3}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
    <binding template='TileWide'>
      <text id='1'>{0} {1} {3}</text>
      <text id='2' hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
  </visual>
</tile>";
        private const string ToastTemplateXml = @"<toast>
  <visual>
    <binding template='ToastGeneric'>
      <text>{0} {1} 最新物流动态</text>
      <text hint-wrap='true'>{2}</text>
    </binding>
  </visual>
</toast>";
        private static Dictionary<string, string> comDir
        {
            get
            {
                var comdir = new Dictionary<string, string>();
                comdir.Add("EMS快递", "ems");
                comdir.Add("申通快递", "shentong");
                comdir.Add("圆通快递", "yuantong");
                comdir.Add("中通快递", "zhongtong");
                comdir.Add("汇通快递", "huitong");
                comdir.Add("天天快递", "tiantian");
                comdir.Add("韵达快递", "yunda");
                comdir.Add("顺丰快递", "shunfeng");
                comdir.Add("宅急送快递", "zhaijisong");
                comdir.Add("中国邮政", "pingyou");
                return comdir;
            }
        }
        private class KD_Model
        {
            public string name { get; set; }
            public string num { get; set; }
            public string com { get; set; }
            public string com_cn
            {
                get
                {
                    foreach (var item in comDir)
                    {
                        if (com == item.Value) return item.Key;
                    }
                    return "";
                }
            }
            public Color color { get; set; }
            public string color_hex
            {
                get
                {
                    return color.ToString();
                }
            }
            public Windows.Data.Json.JsonObject GetJsonObj()
            {
                var obj = new Windows.Data.Json.JsonObject();
                obj.Add("name", Windows.Data.Json.JsonValue.CreateStringValue(name));
                obj.Add("num", Windows.Data.Json.JsonValue.CreateStringValue(num));
                obj.Add("com", Windows.Data.Json.JsonValue.CreateStringValue(com));
                obj.Add("color", Windows.Data.Json.JsonValue.CreateStringValue(color.A + "," + color.R + "," + color.G + "," + color.B));
                return obj;
            }
            public KD_Model(string _name, string _num, string _com, string _color)
            {
                name = _name;
                num = _num;
                com = _com;
                var colors = _color.Split(',');
                color = Color.FromArgb(byte.Parse(colors[0]), byte.Parse(colors[1]), byte.Parse(colors[2]), byte.Parse(colors[3]));
            }
            public KD_Model(string _name, string _num, string _com)
            {
                name = _name;
                num = _num;
                com = _com;
            }
            public KD_Model() { }
        }
        private class KD_Model_Net : KD_Model
        {
            public KD_Model_Net() { }
            public KD_Model_Net(KD_Model _kd)
            {
                name = _kd.name;
                num = _kd.num;
                com = _kd.com;
                color = _kd.color;
            }
            public string message { get; set; }
            public string ischeck { get; set; }
            public string state
            {
                get
                {
                    if (message == "ok")
                    {
                        if (ischeck == "0")
                        {
                            return "包裹运送中...";
                        }
                        else
                        {
                            return "已签收";
                        }
                    }
                    else
                    {
                        return "运单编号异常,无法获取信息";
                    }
                }
            }
            public Data newdata
            {
                get
                {
                    if (data != null && data.Count > 0)
                    {
                        return data[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public List<Data> data { get; set; }
            public class Data
            {
                public string time { get; set; }
                public string context { get; set; }
                public string time_s
                {
                    get
                    {
                        var newtime = DateTime.Now - DateTime.Parse(time);
                        if (newtime.Days == 0)
                        {
                            return newtime.Hours + "小时前";
                        }
                        else
                        {
                            return newtime.Days + "天" + newtime.Hours + "小时前";
                        }
                    }
                }
            }
            public async Task Load()
            {
                try
                {
                        var httpclient = new System.Net.Http.HttpClient();
                        var json = await httpclient.GetStringAsync("http://www.kuaidi100.com/query?type=" + com + "&postid=" + num);
                        var obj = Windows.Data.Json.JsonObject.Parse(json);
                        message = obj.GetNamedString("message");
                        if (message == "ok")
                        {
                            ischeck = obj.GetNamedString("ischeck");
                            var array = obj.GetNamedArray("data");
                            data = new List<Data>();
                            foreach (var item in array)
                            {
                                data.Add(new Data() { time = item.GetObject()["time"].GetString(), context = item.GetObject()["context"].GetString() });
                            }
                        }
                }
                catch (Exception)
                {

                }
            }
        }
        private async Task<List<KD_Model>> GetKD()
        {
            var list = new List<KD_Model>();
            try
            {
                var kd_file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("kd_file", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var json = await Windows.Storage.FileIO.ReadTextAsync(kd_file);
                var array = Windows.Data.Json.JsonArray.Parse(json);
                foreach (var item in array)
                {
                    list.Add(new KD_Model(item.GetObject()["name"].GetString(), item.GetObject()["num"].GetString(), item.GetObject()["com"].GetString(), item.GetObject()["color"].GetString()));
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }
        private async Task<bool> isNew(KD_Model_Net kd)
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var tempkdfile = await localFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var array = Windows.Data.Json.JsonArray.Parse(await Windows.Storage.FileIO.ReadTextAsync(tempkdfile));
                foreach (var item in array)
                {
                    if (item.GetObject().GetNamedString("num") == kd.num)
                    {
                        if (item.GetObject().GetNamedString("time") == kd.newdata.time)
                        {
                            return false;
                        }
                        else
                        {
                            await UpdataNum(kd);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task UpdataNum(KD_Model_Net kd)
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var tempkdfile = await localFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var array = Windows.Data.Json.JsonArray.Parse(await Windows.Storage.FileIO.ReadTextAsync(tempkdfile));
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i].GetObject().GetNamedString("num") == kd.num)
                    {
                        array[i].GetObject().Remove("time");
                        array[i].GetObject().Add("time", Windows.Data.Json.JsonValue.CreateStringValue(kd.newdata.time));
                        await Windows.Storage.FileIO.WriteTextAsync(tempkdfile, array.ToString());
                        return;
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
