using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Entities
{

    [Table("MoviesUsers")]
    public class MovieUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int MovieId { get; set; }

        [Required]
        public Guid UserId { get; set; }

    }
}
