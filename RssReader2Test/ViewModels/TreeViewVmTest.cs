using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using RssReader2.Models;
using RssReader2.ViewModels;

namespace RssReader2Test.ViewModels
{
    [TestFixture]
    public class TreeViewVmTest
    {
        [Test]
        public void FindSelectedItemTest_選択中のアイテムが含まれる場合()
        {
            var items = new ObservableCollection<IWebSiteTreeViewItem>()
            {
                new WebSite(),
                new WebSite()
                {
                    Children = new List<IWebSiteTreeViewItem>()
                    {
                        new WebSite(),
                        new WebSite() { IsSelected = true, Title = "expected value", },
                    },
                },
                new WebSite(),
            };

            var vm = new TreeViewVm(null, null, null);
            Assert.That(vm.FindSelectedItem(items).Name, Is.EqualTo("expected value"));
        }

        [Test]
        public void FindSelectedItemTest_選択中のアイテムが含まれない場合()
        {
            var items = new ObservableCollection<IWebSiteTreeViewItem>()
            {
                new WebSite(),
                new WebSite()
                {
                    Children = new List<IWebSiteTreeViewItem>()
                    {
                        new WebSite(),
                        new WebSite(),
                    },
                },
                new WebSite(),
            };

            var vm = new TreeViewVm(null, null, null);
            Assert.IsNull(vm.FindSelectedItem(items));
        }

        [Test]
        public void FindSelectedItemTest_選択中のアイテムが最上層にある場合()
        {
            var items = new ObservableCollection<IWebSiteTreeViewItem>()
            {
                new WebSite(),
                new WebSite() { IsSelected = true, Title = "expected value", },
                new WebSite()
                {
                    Children = new List<IWebSiteTreeViewItem>()
                    {
                        new WebSite(),
                        new WebSite(),
                    },
                },
                new WebSite(),
            };

            var vm = new TreeViewVm(null, null, null);
            Assert.That(vm.FindSelectedItem(items).Name, Is.EqualTo("expected value"));
        }
    }
}