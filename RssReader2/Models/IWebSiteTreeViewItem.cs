using System.Collections.Generic;

namespace RssReader2.Models
{
    public interface IWebSiteTreeViewItem
    {
        public string Name { get; }

        public IEnumerable<IWebSiteTreeViewItem> Children { get; set; }

        public bool IsSelected { get; set; }
    }
}