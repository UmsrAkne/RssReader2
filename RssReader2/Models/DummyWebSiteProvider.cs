using System.Collections.Generic;
using System.Linq;

namespace RssReader2.Models
{
    public class DummyWebSiteProvider : IWebSiteProvider
    {
        public IEnumerable<WebSite> GetAllWebSites()
        {
            return Enumerable.Range(1, 20).Select(i => new WebSite()
            {
                Title = $"webSite name{i}",
                Url = $"https://webSiteURL.{i}/",
                Children = new List<IWebSiteTreeViewItem>(),
                Id = i,
            });
        }
    }
}