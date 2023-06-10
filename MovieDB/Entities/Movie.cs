using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Entities
{
    [Table("Movies")]
    public class Movie
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(30)]
        [Required]
        public string Title { get; set; }

        [StringLength(30)]
        [Required]
        public string Producer { get; set; }

        [StringLength(30)]
        [Required]
        public string Director { get; set; }

        [StringLength(30)]
        [Required]
        public string MusicDirector { get; set; }

        [StringLength(30)]
        [Required]
        public string ProduceIn { get; set; }
        
        [Required]
        public byte[] Image { get; set; }

    }
}

