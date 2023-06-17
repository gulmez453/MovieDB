using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MovieDB.Entities;

namespace MovieDB.Models
{
    public class UserRate
    {

        [Required]
        public User User { get; set; }

        [Required]
        public Rate Rate { get; set; }


    }
}
