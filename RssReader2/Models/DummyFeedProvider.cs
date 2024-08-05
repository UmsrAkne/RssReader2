using System;
using System.Collections.Generic;
using System.Linq;

namespace RssReader2.Models
{
    public class DummyFeedProvider : IFeedProvider
    {
        private IEnumerable<Feed> feeds;

        public DummyFeedProvider()
        {
            AddDummies(0);
        }

        public IEnumerable<Feed> GetAllFeeds()
        {
            return feeds;
        }

        public IEnumerable<Feed> GetFeedsByWebSiteId(int id)
        {
            if (feeds.All(f => f.ParentSiteId != id))
            {
                AddDummies(id);
            }

            return feeds.Where(f => f.ParentSiteId == id);
        }

        public IEnumerable<Feed> GetFeedsByWebSiteId(int id, int pageSize, int pageNumber)
        {
            if (feeds.All(f => f.ParentSiteId != id))
            {
                AddDummies(id);
            }

            // データをページング
            var items = feeds.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return items;
        }

        public int GetFeedCountByWebSiteId(int id)
        {
            return feeds.Count(f => f.ParentSiteId == id);
        }

        private void AddDummies(int siteId)
        {
            var list = new List<Feed>();

            for (var i = 0; i < 250; i++)
            {
                list.Add(
                    new Feed
                    {
                        IsRead = false,
                        Id = 1 + i,
                        ParentSiteId = siteId,
                        PublishedAt = DateTime.Now.Add(TimeSpan.FromMinutes(i)),
                        Description = $"article description no.{i}",
                        Title = $"article title no.{i} (webSiteId = {siteId})",
                        Url = $"https://dummyUrl/notExists/articleNumber_{i}",
                    });
            }

            feeds = list;
        }
    }
}