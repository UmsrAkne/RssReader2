using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssReader2.Models
{
    public class WebSiteGroup : IWebSiteTreeViewItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        ///     サイトの表示名
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public IEnumerable<IWebSiteTreeViewItem> Children { get; set; } = new List<IWebSiteTreeViewItem>();

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public bool IsGroup { get; set; } = true;

        [NotMapped]
        public bool HasUnreadItem { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}