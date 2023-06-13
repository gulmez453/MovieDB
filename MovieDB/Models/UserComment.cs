using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MovieDB.Entities;

namespace MovieDB.Models
{
    public class UserComment
    {

        [Required]
        public User User { get; set; }

        [Required]
        public Comment Comment { get; set; }


    }
}
