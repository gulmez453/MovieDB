using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail is required")]
        [StringLength(30, ErrorMessage="E-mail can be max 30 characters")]
        public string email { get; set; }

        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage="Password can be min 30 characters")]
        [MaxLength(16, ErrorMessage = "Password can be max 16 characters")]
        public string Password { get; set; }
    }
}
