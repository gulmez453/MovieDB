using System.ComponentModel.DataAnnotations;

namespace MovieDB.Models
{
    public class MovieFilterViewModel
    {
        public FilterViewModel FilterViewModell { get; set; }
        public List<MovieDB.Entities.Movie> MovieViewModel { get; set; }

    }
}
