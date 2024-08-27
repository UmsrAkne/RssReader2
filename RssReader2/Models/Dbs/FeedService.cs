using System;
using System.Collections.Generic;
using System.Linq;

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

        public IQueryable<Feed> GetFeedsByWebSiteId(int id)
        {
            return feedRepository.GetAll().Where(f => f.ParentSiteId == id);
        }

        public IEnumerable<Feed> GetFeedsByWebSiteId(int id, int pageSize, int pageNumber, IEnumerable<NgWord> ngWords)
        {
            var l = GetFeedsByWebSiteId(id)
                .OrderByDescending(f => f.PublishedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            NgWordValidation(l, ngWords);
            return l;
        }

        public IEnumerable<Feed> GetUnreadFeedsByWebSiteId(int id, int pageSize, int pageNumber, IEnumerable<NgWord> ngWords)
        {
            var l = GetFeedsByWebSiteId(id)
                .OrderByDescending(f => f.PublishedAt)
                .Where(f => !f.IsRead)
                .Take(pageSize)
                .ToList();

            NgWordValidation(l, ngWords);
            return l;
        }

        public int GetFeedCountByWebSiteId(int id)
        {
            return feedRepository.GetAll().Count(f => f.ParentSiteId == id);
        }

        public Feed GetFeedById(int id)
        {
            return feedRepository.GetById(id);
        }

        public void AddFeed(Feed feed)
        {
            feedRepository.Add(feed);
        }

        /// <summary>
        /// フィードを追加します。
        /// 既存のフィードと一致するフィードはフィルタリングされ、新たに追加されるフィードのリストのみをフィードリポジトリに追加します。
        /// 各フィードのタイトルと説明が含まれているNGワードのリストに対して確認され、NGワードが含まれている場合、フィードのContainsNgWordプロパティが設定されます。
        /// 最終的に、フィードの最終検証日が現在の日時に設定されます。
        /// </summary>
        /// <param name="feeds">追加したいフィードのコレクションです。</param>
        /// <param name="ngWords">フィードのタイトルまたは説明に含まれていないことを確認したいNGワードのコレクションです。</param>
        public void AddFeeds(IEnumerable<Feed> feeds, IEnumerable<NgWord> ngWords)
        {
            var feedList = feeds.ToList();

            if (!feedList.Any())
            {
                return;
            }

            var ngWordList = ngWords.ToList();
            var all = feedRepository.GetAll();

            // 追加されるフィードの中で一番古い投稿日時を取得しておく。
            // all からこれよりも古いフィードを除外できる。
            var oldestPublishDate = feedList.Min(f => f.PublishedAt);

            var groupedAll = all
                .Where(a => a.PublishedAt >= oldestPublishDate)
                .GroupBy(a => a.ParentSiteId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var filteredFeeds = feedList.Where(f =>
            {
                if (!groupedAll.TryGetValue(f.ParentSiteId, out var relatedFeeds))
                {
                    // groupedAll の中に、対応するグループがない場合はフィルタに値を含める。
                    return true;
                }

                return !relatedFeeds.Any(a => a.AreEqual(f));
            }).ToList();

            foreach (var feed in filteredFeeds)
            {
                feed.ContainsNgWord =
                    ngWordList.Any(ngWord => feed.Title.Contains(ngWord.Word)) ||
                    ngWordList.Any(ngWord => feed.Description.Contains(ngWord.Word));

                feed.LastValidationDate = DateTime.Now;
            }

            feedRepository.AddRange(filteredFeeds);
        }

        /// <summary>
        /// 特定のWebSite IDに関連付けられているフィードを取得します。NGワードを含むフィードはフィルタされます。
        /// </summary>
        /// <param name="webSite">フィードを取得するWebSiteのインスタンス</param>
        /// <param name="ngWords">フィードからフィルタリングするための、NGワードのコレクション</param>
        /// <returns>NGワードを含まないフィードのコレクション</returns>
        public IEnumerable<Feed> GetFeedsByWebSiteId(WebSite webSite, IEnumerable<NgWord> ngWords)
        {
            var ngWordList = ngWords.ToList();
            var latestNgWordUpdated = ngWordList.Max(w => w.LastUpdated);
            var allFeeds = GetAllFeeds()
                .Where(f => f.ParentSiteId == webSite.Id).ToList();

            var notValidation = allFeeds.Where(f => f.LastValidationDate < latestNgWordUpdated);
            NgWordValidation(notValidation, ngWordList);

            return allFeeds.Where(f => !f.ContainsNgWord);
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

        /// <summary>
        /// 入力されたサイトIDの中からNGワードを含むフィードを抽出し、既読にします。
        /// </summary>
        /// <param name="siteId">対象とするサイトID</param>
        /// <remarks>
        /// リポジトリからサイトIDで検索し、NGワードを含み、未読のフィードを抽出してそれをアップデートします。<br/>
        /// このメソッドはデータの更新のみを実行します。NGワードのバリデーションが完了した後にコールしてください。
        /// </remarks>
        public void MarkNgWordFeedsAsReadByWebSiteId(int siteId)
        {
            var feeds = feedRepository.GetAll()
                .Where(f => f.ParentSiteId == siteId)
                .Where(f => f.ContainsNgWord)
                .Where(f => !f.IsRead);

            if (!feeds.Any())
            {
                return;
            }

            foreach (var feed in feeds)
            {
                feed.IsRead = true;
            }

            feedRepository.UpdateRange(feeds);
        }

        private void NgWordValidation(IEnumerable<Feed> feeds, IEnumerable<NgWord> ngWords)
        {
            var ngWordList = ngWords.ToList();
            var feedList = feeds.ToList();

            foreach (var feed in feedList)
            {
                feed.ContainsNgWord =
                    ngWordList.Any(ngWord => feed.Title.Contains(ngWord.Word)) ||
                    ngWordList.Any(ngWord => feed.Description.Contains(ngWord.Word));

                feed.LastValidationDate = DateTime.Now;
            }

            feedRepository.UpdateRange(feedList);
        }
    }
}