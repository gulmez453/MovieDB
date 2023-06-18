using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class RegisterViewModel
    {   //register data model with annotation limits and error message
        [Required(ErrorMessage = "E-mail is required")]
        [StringLength(30, ErrorMessage = "E-mail can be max 30 characters")]
        public string email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30, ErrorMessage = "Name can be max 30 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password can be min 30 characters")]
        [MaxLength(16, ErrorMessage = "Password can be max 16 characters")]
        public string Password { get; set; }

        
        [Required(ErrorMessage = "Re-Password is required")]
        [MinLength(6, ErrorMessage = "Re-Password can be min 30 characters")]
        [MaxLength(16, ErrorMessage = "Re-Password can be max 16 characters")]
        //compare repassword with password
        [Compare(nameof(Password))] 
        public string RePassword { get; set; }
    }
}
