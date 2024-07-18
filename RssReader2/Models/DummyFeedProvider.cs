using System;
using System.Collections.Generic;

namespace RssReader2.Models
{
    public class DummyFeedProvider : IFeedProvider
    {
        private IEnumerable<Feed> feeds;

        public DummyFeedProvider()
        {
            var l = new List<Feed>();

            for (var i = 0; i < 250; i++)
            {
                l.Add(
                    new Feed
                    {
                        IsRead = false,
                        Id = 1 + i,
                        ParentSiteId = 0,
                        DateTime = DateTime.Now.Add(TimeSpan.FromMinutes(i)),
                        Description = $"article description no.{i}",
                        Title = $"article title no.{i}",
                        Url = $"https://dummyUrl/notExists/articleNumber_{i}",
                    });
            }

            feeds = l;
        }

        public IEnumerable<Feed> GetAllFeeds()
        {
            return feeds;
        }
    }
}