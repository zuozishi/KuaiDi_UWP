using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace KuaiDi.Class
{
    public class KD_Class
    {
        public static bool isInternetAvailable
        {
            get
            {
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (InternetConnectionProfile == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static Dictionary<string, string> comDir
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
        public static async Task<List<KD_Model>> Get()
        {
            var list= new List<KD_Model>();
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
        public static async Task Add(KD_Model kd)
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
            int num = new Random().Next(0, ColorList.Count);
            kd.color = ColorList[num];
            var kd_file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("kd_file", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var tempkdfile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var temparray = new Windows.Data.Json.JsonArray();
            if (await Windows.Storage.FileIO.ReadTextAsync(tempkdfile) != "")
            {
                temparray = Windows.Data.Json.JsonArray.Parse(await Windows.Storage.FileIO.ReadTextAsync(tempkdfile));
            }
            var tempobj = new Windows.Data.Json.JsonObject();
            tempobj.Add("num", Windows.Data.Json.JsonValue.CreateStringValue(kd.num));
            tempobj.Add("time", Windows.Data.Json.JsonValue.CreateStringValue("0"));
            temparray.Add(tempobj);
            await Windows.Storage.FileIO.WriteTextAsync(tempkdfile, temparray.ToString());
            var json = await Windows.Storage.FileIO.ReadTextAsync(kd_file);
            if (json != "")
            {
                var array = Windows.Data.Json.JsonArray.Parse(json);
                array.Add(kd.GetJsonObj());
                json = array.ToString();
            }else
            {
                var obj = new Windows.Data.Json.JsonArray();
                obj.Add(kd.GetJsonObj());
                json = obj.ToString();
            }
            await Windows.Storage.FileIO.WriteTextAsync(kd_file, json);
        }
        public static async Task Remove(KD_Model kd)
        {
            try
            {
                var tempkdfile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var temparray = new Windows.Data.Json.JsonArray();
                for (int i = 0; i < temparray.Count; i++)
                {
                    if(temparray[i].GetObject().GetNamedString("num") == kd.num)
                    {
                        temparray.RemoveAt(i);
                        await Windows.Storage.FileIO.WriteTextAsync(tempkdfile, temparray.ToString());
                        break;
                    }
                }
                var kd_file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("kd_file", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var json = await Windows.Storage.FileIO.ReadTextAsync(kd_file);
                var array = Windows.Data.Json.JsonArray.Parse(json);
                for (int i = 0; i < array.Count; i++)
                {
                    if (kd.num == array[i].GetObject()["num"].GetString())
                    {
                        array.RemoveAt(i);
                    }
                }
                await Windows.Storage.FileIO.WriteTextAsync(kd_file, array.ToString());
            }
            catch (Exception)
            {
                
            }
       }
        public static async Task Clear()
        {
            var kd_file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("kd_file", Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.WriteTextAsync(kd_file,"");
            var tempkdfile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }
        public static async Task<bool> CanAdd(KD_Model kd)
        {
            try
            {
                bool canadd = true;
                var kd_file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("kd_file", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var json = await Windows.Storage.FileIO.ReadTextAsync(kd_file);
                var array = Windows.Data.Json.JsonArray.Parse(json);
                for (int i = 0; i < array.Count; i++)
                {
                    if (kd.num == array[i].GetObject()["num"].GetString())
                    {
                        canadd = false;
                    }
                }
                return canadd;
            }
            catch (Exception)
            {
                return true;
            }
        }
        public class KD_Model
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
                obj.Add("color", Windows.Data.Json.JsonValue.CreateStringValue(color.A+","+color.R+","+color.G+","+color.B));
                return obj;
            }
            public KD_Model(string _name,string _num,string _com,string _color)
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
        public class KD_Model_Net : KD_Model, INotifyPropertyChanged
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
                    }else
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
            public List<Data> data_without0
            {
                get
                {
                    if (data != null && data.Count > 0)
                    {
                        var list = new List<Data>();
                        for (int i = 1; i < data.Count; i++)
                        {
                            list.Add(data[i]);
                        }
                        return list;
                    }
                    else
                    {
                        return new List<Data>();
                    }
                }
            }
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
                    if (isInternetAvailable)
                    {
                        MoreListVisibility = "Collapsed";
                        var httpclient = new System.Net.Http.HttpClient();
                        var json = await httpclient.GetStringAsync("http://www.kuaidi100.com/query?type="+com+"&postid="+num);
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
                            var tempkdfile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("tempkdfile", Windows.Storage.CreationCollisionOption.OpenIfExists);
                            var temparray = Windows.Data.Json.JsonArray.Parse(await Windows.Storage.FileIO.ReadTextAsync(tempkdfile));
                            for (int i = 0; i < temparray.Count; i++)
                            {
                                if (temparray[i].GetObject().GetNamedString("num") == num)
                                {
                                    temparray[i].GetObject().Remove("time");
                                    temparray[i].GetObject().Add("time", Windows.Data.Json.JsonValue.CreateStringValue(newdata.time));
                                    await Windows.Storage.FileIO.WriteTextAsync(tempkdfile, temparray.ToString());
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            public string MoreListVisibility { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public class Theme_Class
    {
        public static void ChangeThemeColor(Color color)
        {
            var lightTheme = Application.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
            (lightTheme["KD_main_Color"] as SolidColorBrush).Color = color;
            var darkTheme = Application.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
            (darkTheme["KD_main_Color"] as SolidColorBrush).Color = color;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.Black;
                statusBar.BackgroundColor = color;
                statusBar.BackgroundOpacity = 100;
            }
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = color;
            titleBar.ButtonBackgroundColor = color;
        }
        public static void ChangeThemeColor()
        {
            var lightTheme = Application.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
            (lightTheme["KD_main_Color"] as SolidColorBrush).Color = (lightTheme["KD_system_Color"] as SolidColorBrush).Color;
            var darkTheme = Application.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
            (darkTheme["KD_main_Color"] as SolidColorBrush).Color = (lightTheme["KD_system_Color"] as SolidColorBrush).Color;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.Black;
                statusBar.BackgroundColor = (lightTheme["KD_system_Color"] as SolidColorBrush).Color;
                statusBar.BackgroundOpacity = 100;
            }
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = (lightTheme["KD_system_Color"] as SolidColorBrush).Color;
            titleBar.ButtonBackgroundColor = (lightTheme["KD_system_Color"] as SolidColorBrush).Color;
        }
    }
}
