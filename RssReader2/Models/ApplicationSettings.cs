using System.IO;
using System.Text.Json;
using Prism.Mvvm;

namespace RssReader2.Models
{
    public class ApplicationSettings : BindableBase
    {
        public readonly static string SettingFileName = "settings.json";
        private int autoUpdateInterval = 90;
        private bool autoUpdateEnabled;

        public int AutoUpdateInterval
        {
            get => autoUpdateInterval;
            set => SetProperty(ref autoUpdateInterval, value);
        }

        public bool AutoUpdateEnabled
        {
            get => autoUpdateEnabled;
            set => SetProperty(ref autoUpdateEnabled, value);
        }

        /// <summary>
        /// 指定したファイルパスからアプリの設定を読み取って取得します。
        /// </summary>
        /// <param name="filePath">読みに行くパスを指定します。</param>
        /// <returns>読み取ったアプリの設定です。指定パスにファイルが存在しない場合、 default を返します。</returns>
        public static ApplicationSettings LoadJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new ApplicationSettings();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ApplicationSettings>(json);
        }

        public void SaveToJson(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }
    }
}