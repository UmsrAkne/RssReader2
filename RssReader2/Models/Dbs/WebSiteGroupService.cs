using System;
using System.Collections.Generic;

namespace RssReader2.Models.Dbs
{
    public class WebSiteGroupService
    {
        private readonly IRepository<WebSiteGroup> webSiteGroupRepository;

        public WebSiteGroupService(IRepository<WebSiteGroup> webSiteGroupRepository)
        {
            this.webSiteGroupRepository = webSiteGroupRepository;
        }

        public IEnumerable<WebSiteGroup> GetAllWebSiteGroups()
        {
            return webSiteGroupRepository.GetAll();
        }

        public WebSiteGroup GetWebSiteGroupById(int id)
        {
            return webSiteGroupRepository.GetById(id);
        }

        public void AddWebSiteGroup(WebSiteGroup webSite)
        {
            webSiteGroupRepository.Add(webSite);
        }

        public void AddWebSiteGroups(IEnumerable<WebSiteGroup> webSites)
        {
            webSiteGroupRepository.AddRange(webSites);
        }

        public void UpdateWebSiteGroup(WebSiteGroup webSite)
        {
            webSiteGroupRepository.Update(webSite);
        }

        public void DeleteWebSiteGroup(int id)
        {
            var webSite = webSiteGroupRepository.GetById(id);
            if (webSite == null)
            {
                throw new ArgumentException("not found.");
            }

            webSiteGroupRepository.Delete(id);
        }
    }
}