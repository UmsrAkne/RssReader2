using System;
using System.Collections.Generic;
using System.Linq;

namespace RssReader2.Models.Dbs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // DI で注入するため、生成されない。
    public class WebSiteService : IWebSiteProvider
    {
        private readonly IRepository<WebSite> webSiteRepository;

        public WebSiteService(IRepository<WebSite> webSiteRepository)
        {
            this.webSiteRepository = webSiteRepository;
        }

        public IEnumerable<WebSite> GetAllWebSites()
        {
            return webSiteRepository.GetAll();
        }

        public WebSite GetWebSiteById(int id)
        {
            return webSiteRepository.GetById(id);
        }

        /// <summary>
        /// AddWebSiteメソッドは、新しいWebサイトをシステムに追加します。
        /// </summary>
        /// <remarks>
        /// Webサイトは、タイトルとURLが空でなく、URLが既存のWebサイトと重複しない場合に追加されます。
        /// </remarks>
        /// <param name="webSite">追加するWebSiteオブジェクト。</param>
        public void AddWebSite(WebSite webSite)
        {
            var isEnabledItem =
                !string.IsNullOrWhiteSpace(webSite.Title) && !string.IsNullOrWhiteSpace(webSite.Url);

            var all = GetAllWebSites();
            if (all.All(w => w.Url != webSite.Url) && isEnabledItem)
            {
                webSiteRepository.Add(webSite);
            }
        }

        public void AddWebSites(IEnumerable<WebSite> webSites)
        {
            webSiteRepository.AddRange(webSites);
        }

        public void UpdateWebSite(WebSite webSite)
        {
            webSiteRepository.Update(webSite);
        }

        public void DeleteWebSite(int id)
        {
            var webSite = webSiteRepository.GetById(id);
            if (webSite == null)
            {
                throw new ArgumentException("WebSite not found.");
            }

            webSiteRepository.Delete(id);
        }
    }
}