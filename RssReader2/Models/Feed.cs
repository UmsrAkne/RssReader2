using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace RssReader2.Models
{
    public class Feed : BindableBase
    {
        private bool isRead;
        private bool isMarked;
        private int lineNumber;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ParentSiteId { get; set; }

        /// <summary>
        /// このフィードの記事が公開された日時。
        /// </summary>
        [Required]
        public DateTime PublishedAt { get; set; }

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        [Required]
        public string Description { get; set; } = string.Empty;

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        [Required]
        public string Title { get; set; } = string.Empty;

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        public bool IsMarked { get => isMarked; set => SetProperty(ref isMarked, value); }

        /// <summary>
        /// このフィードが Ngワード を含んでいるかを表します。
        /// </summary>
        [Required]
        public bool ContainsNgWord { get; set; }

        /// <summary>
        /// このフィードが Ngワード を含んでいるかを確認した日付です。
        /// </summary>
        [Required]
        public DateTime LastValidationDate { get; set; }

        [Required]
        public bool IsRead { get => isRead; set => SetProperty(ref isRead, value); }

        [NotMapped]
        public int LineNumber { get => lineNumber; set => SetProperty(ref lineNumber, value); }

        public bool AreEqual(Feed another)
        {
            return Title == another.Title && Url == another.Url;
        }
    }
}