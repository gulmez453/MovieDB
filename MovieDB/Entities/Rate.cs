using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Entities
{
    [Table("Rates")]
    public class Rate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieUserId { get; set; }

        [Required]
        public int RateNum { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime CreatedAt { get; set; }


    }
}
