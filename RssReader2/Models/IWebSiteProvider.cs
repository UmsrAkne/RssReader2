using System.Collections.Generic;

namespace RssReader2.Models
{
    public interface IWebSiteProvider
    {
        public IEnumerable<WebSite> GetAllWebSites();
    }
}