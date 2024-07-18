using System;
using System.ComponentModel.DataAnnotations;
using Prism.Mvvm;

namespace RssReader2.Models
{
    public class Feed : BindableBase
    {
        private bool isRead;

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ParentSiteId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        public bool IsRead { get => isRead; set => SetProperty(ref isRead, value); }
    }
}