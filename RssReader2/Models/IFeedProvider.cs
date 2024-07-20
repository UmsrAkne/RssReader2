using System.Collections.Generic;

namespace RssReader2.Models
{
    public interface IFeedProvider
    {
        IEnumerable<Feed> GetAllFeeds();
    }
}