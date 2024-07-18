using System.Linq;
using NUnit.Framework;
using RssReader2.Models;

namespace RssReader2Test.Models
{
    [TestFixture]
    public class FeedReaderTest
    {
        [Test]
        public void ConvertToFeedsTest()
        {
            const string rssText = @"<rss version=""2.0"">"
                                   + "<channel>"
                                   + "    <title>Example RSS Feed</title>"
                                   + "    <link>http://www.example.com</link>"
                                   + "    <description>This is an example RSS feed</description>"
                                   + "    <pubDate>Mon, 06 Sep 2021 00:00:00 GMT</pubDate>"
                                   + "    <item>"
                                   + "        <title>Example entry</title>"
                                   + "        <link>http://www.example.com/example-entry</link>"
                                   + "        <description>This is an example entry in the RSS feed</description>"
                                   + "        <pubDate>Mon, 06 Sep 2021 00:00:00 GMT</pubDate>"
                                   + "    </item>"
                                   + "</channel>"
                                   + "</rss>";

            var feeds = FeedReader.ConvertToFeeds(rssText);
            var feed = feeds.First();

            Assert.That(feed.Title, Is.EqualTo("Example entry"));
            Assert.That(feed.Url, Is.EqualTo("http://www.example.com/example-entry"));
            Assert.That(feed.Description, Is.EqualTo("This is an example entry in the RSS feed"));
        }
    }
}