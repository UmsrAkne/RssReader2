using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RssReader2.Models
{
    public class NgWord
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Word { get; set; } = string.Empty;

        [NotMapped]
        public int Index { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}