using System;
using System.IO;
using Prism.Mvvm;

namespace RssReader2.Models
{
    public class Logger : BindableBase
    {
        // ReSharper disable once ArrangeModifiersOrder
        private const string LogFilePath = "log.txt";

        private static string message = string.Empty;

        public string Message
        {
            get => message;
            private set => SetProperty(ref message, value);
        }

        /// <summary>
        /// 入力したメッセージを、外部のテキストファイルに書き込み、Message プロパティに追加します。
        /// </summary>
        /// <param name="msg">ログに追記したいテキストを入力します。</param>
        public void Log(string msg)
        {
            try
            {
                var m = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}";
                Message = $"{m}{Environment.NewLine}{Message}";

                using var writer = new StreamWriter(LogFilePath, true);
                writer.WriteLine(m);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ログの書き込み中にエラーが発生しました: {ex.Message}");
            }
        }
    }
}