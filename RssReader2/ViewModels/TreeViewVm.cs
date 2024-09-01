using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using RssReader2.Models;
using RssReader2.Models.Dbs;

namespace RssReader2.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TreeViewVm : BindableBase
    {
        private ObservableCollection<IWebSiteTreeViewItem> webSiteTreeViewItems;

        public TreeViewVm(IWebSiteProvider webSiteProvider, WebSiteGroupService webSiteGroupService, FeedService feedService)
        {
            WebSiteProvider = webSiteProvider;
            WebSiteGroupService = webSiteGroupService;
            FeedService = feedService;
        }

        public ObservableCollection<IWebSiteTreeViewItem> WebSiteTreeViewItems
        {
            get => webSiteTreeViewItems;
            set => SetProperty(ref webSiteTreeViewItems, value);
        }

        public DelegateCommand UpdateUnreadFeedDisplayCommand => new DelegateCommand(() =>
        {
            UpdateHasUnread(SelectedItem);
        });

        public IWebSiteTreeViewItem SelectedItem { get; set; }

        private IWebSiteProvider WebSiteProvider { get; set; }

        private FeedService FeedService { get; set; }

        private WebSiteGroupService WebSiteGroupService { get; set; }

        /// <summary>
        /// 指定された IEnumerable <paramref name="items"/> コレクションから選択された項目を見つけます。
        /// </summary>
        /// <param name="items">検索対象の IWebSiteTreeViewItem オブジェクトのコレクション。</param>
        /// <returns>選択された IWebSiteTreeViewItem を返します。選択された項目が存在しない場合は null を返します。</returns>
        /// <remarks>
        /// このメソッドは再帰的にコレクションを検索し、選択された項目が見つかった時点でそれを返します。
        /// コレクション内の各項目について、まずその項目自身が選択されているかを確認し、
        /// 選択されていない場合はその子項目のコレクションを再帰的に検索します。
        /// </remarks>
        public IWebSiteTreeViewItem FindSelectedItem(IEnumerable<IWebSiteTreeViewItem> items)
        {
            IWebSiteTreeViewItem selectedItem = null;

            foreach (var item in items)
            {
                if (selectedItem != null)
                {
                    return selectedItem;
                }

                if (item.IsSelected)
                {
                    selectedItem = item;
                    break;
                }

                if (item.Children != null)
                {
                    selectedItem = FindSelectedItem(item.Children);
                }
            }

            return selectedItem;
        }

        /// <summary>
        /// 全てのウェブサイト・ウェブサイトグループを取得し、ツリービューを更新します。
        /// </summary>
        public void ReloadTreeViewItems()
        {
            var sites = WebSiteProvider
                .GetAllWebSites()
                .Cast<IWebSiteTreeViewItem>();

            var groups = WebSiteGroupService
                .GetAllWebSiteGroups()
                .Cast<IWebSiteTreeViewItem>();

            WebSiteTreeViewItems = new ObservableCollection<IWebSiteTreeViewItem>(groups.Concat(sites));
            foreach (var w in WebSiteTreeViewItems)
            {
                UpdateHasUnread(w);
            }
        }

        private void UpdateHasUnread(IWebSiteTreeViewItem item)
        {
            if (item is not WebSite w)
            {
                return;
            }

            w.HasUnreadItem = FeedService.HasUnreadFeed(w.Id);
        }
    }
}