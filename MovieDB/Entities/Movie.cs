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
        public string Artists { get; set; }

        [StringLength(30)]
        [Required]
        public string Director { get; set; }

        [StringLength(30)]
        [Required]
        public string Type { get; set; }

        [Required]
        public int ProduceYear { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Minute { get; set; }

        [StringLength(30)]
        [Required]
        public string Description { get; set; }

        [Required]
        public byte[] Image { get; set; }

    }
}

