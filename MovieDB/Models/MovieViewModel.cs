using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieViewModel
    {
        [Required(ErrorMessage = "required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "required")]
        public string Artists { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Director { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "required")]
        public int ProduceYear { get; set; }

        [Required(ErrorMessage = "required")]
        public int Rate { get; set; }

        [Required(ErrorMessage = "required")]
        public int Hour { get; set; }

        [Required(ErrorMessage = "required")]
        public int Minute { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(30, ErrorMessage = "can be max 30 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "required")]
        [Display(Name = "Choose Image")]
        public IFormFile Image { get; set; }

    }
}
