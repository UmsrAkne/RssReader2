using System;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2Test.Models.Dbs
{
    [TestFixture]
    public class FeedManagerTests
    {
        private FeedService feedManager;
        private Mock<IRepository<Feed>> feedRepositoryMock;

        [SetUp]
        public void Setup()
        {
            feedRepositoryMock = new Mock<IRepository<Feed>>();
            feedManager = new FeedService(feedRepositoryMock.Object);
        }

        [Test]
        public void AddFeeds_AddsOnlyNewFeeds()
        {
            // 追加されるフィードとNGワードをセットアップします。
            var feedsToAdd = new List<Feed>
            {
                new() { ParentSiteId = 1, Title = "TestFeed1", Description = "TestDescription1", },
                new() { ParentSiteId = 2, Title = "TestFeed2", Description = "TestDescription2", },
            };

            var existingFeeds = new List<Feed>
            {
                new() { ParentSiteId = 1, Title = "TestFeed1", Description = "TestDescription1", },
            };

            var ngWords = new List<NgWord>
            {
                new() { Word = "bad-word", },
            };

            feedRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingFeeds.AsQueryable);

            // メソッドをテストします。
            feedManager.AddFeeds(feedsToAdd, ngWords);

            // フィード2だけが追加されることを検証します。
            It.Is<IEnumerable<Feed>>(f => f.Single().ParentSiteId == 2);

            // IRepository.AddRange() が一度だけ呼び出されていることを確認します。
            feedRepositoryMock.Verify(x => x.AddRange(It.IsAny<IEnumerable<Feed>>()), Times.Once);
        }

        [Test]
        public void AddFeeds_ExcludesFeedsContainingNgWords()
        {
            // 追加されるフィードとNGワードをセットアップします。
            var feedsToAdd = new List<Feed>
            {
                new() { ParentSiteId = 1, Title = "TestFeed1", Description = "TestDescription1", },
                new() { ParentSiteId = 2, Title = "TestFeed2", Description = "TestDescription2", },
                new() { ParentSiteId = 2, Title = "TestFeed3", Description = "TestDescription2 bad-word", },
                new() { ParentSiteId = 2, Title = "TestFeed4-bad-word", Description = "TestDescription2", },
                new() { ParentSiteId = 2, Title = "TestFeed5-bad-word", Description = "TestDescription2 bad-word", },
            };

            var ngWords = new List<NgWord>
            {
                new() { Word = "bad-word", },
            };

            feedRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Feed>().AsQueryable);

            // メソッドをテストします。
            feedManager.AddFeeds(feedsToAdd, ngWords);

            // フィード3以降が除外され、フィード1,2 の２つだけが追加される。
            It.Is<IEnumerable<Feed>>(f =>
                new [] { "TestFeed1", "TestFeed2", }.SequenceEqual(f.Select(feed => feed.Title)));

            // IRepository.AddRange() が一度だけ呼び出されていることを確認します。
            feedRepositoryMock.Verify(x => x.AddRange(It.IsAny<IEnumerable<Feed>>()), Times.Once);
        }
    }
}