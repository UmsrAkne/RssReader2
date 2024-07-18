using System.ComponentModel.DataAnnotations;

namespace RssReader2.Models
{
    public class WebSite
    {
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        ///     サイトの表示名
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///     ウェブサイトの RSS 配信の URL
        /// </summary>
        [Required]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        ///     WebSiteGroup の Id
        /// </summary>
        [Required]
        public int GroupId { get; set; }
    }
}