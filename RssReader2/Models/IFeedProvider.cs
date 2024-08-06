using System.Collections.Generic;

namespace RssReader2.Models
{
    public interface IFeedProvider
    {
        IEnumerable<Feed> GetAllFeeds();

        IEnumerable<Feed> GetFeedsByWebSiteId(int id);

        IEnumerable<Feed> GetFeedsByWebSiteId(int id, int pageSize, int pageNumber);

        int GetFeedCountByWebSiteId(int id);

        void UpdateFeed(Feed feed);
    }
}