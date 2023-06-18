using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieFilterViewModel
    {
        public FilterViewModel FilterViewModel { get; set; }
        public List<MovieDB.Entities.Movie> MovieViewModel { get; set; }
        public int PageNumber { get; set; }
        public int TotalMovies { get; set; }
        public int TotalPages { get; set; }
    }
}
