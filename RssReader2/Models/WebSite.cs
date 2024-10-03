using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace RssReader2.Models
{
    public class WebSite : BindableBase, IWebSiteTreeViewItem
    {
        private bool isSelected;
        private string title = string.Empty;
        private string url = string.Empty;
        private int groupId;
        private bool hasUnreadItem;

        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        ///     サイトの表示名
        /// </summary>
        [Required]

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string Title
        {
            get => title;
            set
            {
                if (SetProperty(ref title, value))
                {
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        ///     ウェブサイトの RSS 配信の URL
        /// </summary>
        [Required]

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string Url { get => url; set => SetProperty(ref url, value); }

        /// <summary>
        ///     WebSiteGroup の Id
        /// </summary>
        [Required]
        public int GroupId { get => groupId; set => SetProperty(ref groupId, value); }

        [NotMapped]
        public string Name => Title;

        [NotMapped]
        public IEnumerable<IWebSiteTreeViewItem> Children { get; set; } = new List<IWebSiteTreeViewItem>();

        [NotMapped]
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        [NotMapped]
        public bool IsGroup { get; set; }

        [NotMapped]
        public bool HasUnreadItem { get => hasUnreadItem; set => SetProperty(ref hasUnreadItem, value); }
    }
}