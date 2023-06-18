using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDB.Entities
{
    [Table("Users")] // for database table name
    public class User
    {
        [Key] // uniqe key
        public Guid Id { get; set; }

        [StringLength(30)]
        [Required]
        public string FullName { get; set; }
        //for unique email/username validation with error
        [Remote("IsUsernameUnique", "Validation", HttpMethod = "POST", ErrorMessage = "Mail adresi zaten kullanılıyor.")]
        [StringLength(30)]
        [Required]
        public string email { get; set; }

        [StringLength(100)]
        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(30)]
        public string Role { get; set; } = "user"; //default role user
    }
}
