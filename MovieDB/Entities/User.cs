using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(30)]
        [Required]
        public string FullName { get; set; }
        [StringLength(30)]
        [Required]
        public string email { get; set; }

        [StringLength(30)]
        [Required]
        public string Password { get; set; }
    }
}
