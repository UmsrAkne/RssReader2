using System;
using System.Collections.Generic;

namespace RssReader2.Models.Dbs
{
    public class WebSiteService
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

        public void AddWebSite(WebSite webSite)
        {
            webSiteRepository.Add(webSite);
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