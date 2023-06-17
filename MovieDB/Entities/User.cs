using Microsoft.AspNetCore.Mvc;
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

        [Remote("IsUsernameUnique", "Validation", HttpMethod = "POST", ErrorMessage = "Mail adresi zaten kullanılıyor.")]
        [StringLength(30)]
        [Required]
        public string email { get; set; }

        [StringLength(100)]
        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(30)]
        public string Role { get; set; } = "user";
    }
}
