using System.Collections.Generic;

namespace RssReader2.Models
{
    public interface IFeedProvider
    {
        IEnumerable<Feed> GetAllFeeds();

        IEnumerable<Feed> GetFeedsByWebSiteId(int id);

        IEnumerable<Feed> GetFeedsByWebSiteId(int id, int pageSize, int pageNumber);

        IEnumerable<Feed> GetFeedsByWebSiteId(int id, int pageSize, int pageNumber, IEnumerable<NgWord> ngWords);

        IEnumerable<Feed> GetUnreadFeedsByWebSiteId(int id, int pageSize, int pageNumber, IEnumerable<NgWord> ngWords);

        int GetFeedCountByWebSiteId(int id);

        void UpdateFeed(Feed feed);
    }
}