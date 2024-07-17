using System;
using System.Collections.Generic;

namespace RssReader2.Models.Dbs
{
    public class FeedService : IFeedProvider
    {
        private readonly IRepository<Feed> feedRepository;

        public FeedService(IRepository<Feed> feedRepository)
        {
            this.feedRepository = feedRepository;
        }

        public IEnumerable<Feed> GetAllFeeds()
        {
            return feedRepository.GetAll();
        }

        public Feed GetFeedById(int id)
        {
            return feedRepository.GetById(id);
        }

        public void AddFeed(Feed feed)
        {
            feedRepository.Add(feed);
        }

        public void AddFeeds(IEnumerable<Feed> feeds)
        {
            feedRepository.AddRange(feeds);
        }

        public void UpdateFeed(Feed feed)
        {
            feedRepository.Update(feed);
        }

        public void DeleteFeed(int id)
        {
            var feed = feedRepository.GetById(id);
            if (feed == null)
            {
                throw new ArgumentException("Feed not found.");
            }

            feedRepository.Delete(id);
        }
    }
}