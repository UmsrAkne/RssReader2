using System.IO;
using System.Text.Json;

namespace RssReader2.Models
{
    public class ApplicationSettings
    {
        public readonly static string SettingFileName = "settings.json";

        public int AutoUpdateInterval { get; set; } = 90;

        public bool AutoUpdateEnabled { get; set; }

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