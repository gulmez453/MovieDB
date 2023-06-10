using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieViewModel
    {
        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Producer { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Director { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string MusicDirector { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string ProduceIn { get; set; }

        [Required(ErrorMessage = "required")]
        [Display(Name = "Choose Image")]
        public IFormFile Image { get; set; }

    }
}
