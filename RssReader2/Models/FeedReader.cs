using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace RssReader2.Models
{
    public class FeedReader
    {
        /// <summary>
        /// 指定した URL から Feed のリストを生成しあｍす。
        /// </summary>
        /// <param name="url">読み込みたいURL。</param>
        /// <returns>読み込んだリストを取得します。読み込みに失敗した場合、空のリストを返します。</returns>
        public static async Task<IEnumerable<Feed>> GetRssFeeds(string url)
        {
            try
            {
                using var client = new HttpClient();
                var rss = await client.GetStringAsync(url);
                return ConvertToFeeds(rss);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }

            return new List<Feed>();
        }

        /// <summary>
        /// 指定したテキストをフィードのリストに変換します。
        /// </summary>
        /// <param name="rssText">読み込みたい XML のテキスト。</param>
        /// <returns>読み込んだリストを取得します。読み込みに失敗した場合、空のリストを返します。</returns>
        public static IEnumerable<Feed> ConvertToFeeds(string rssText)
        {
            var list = new List<Feed>();

            try
            {
                using var reader = XmlReader.Create(new StringReader(rssText));
                var feed = SyndicationFeed.Load(reader);

                if (feed != null)
                {
                    list = feed.Items.Select(f => new Feed
                    {
                        DateTime = f.PublishDate.DateTime,
                        Description = f.Summary.Text,
                        Title = f.Title.Text,
                        Url = f.Links.FirstOrDefault() != null ? f.Links.First().Uri.ToString() : string.Empty,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return list;
        }
    }
}