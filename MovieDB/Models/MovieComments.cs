using MovieDB.Entities;

namespace MovieDB.Models
{
    public class MovieComments
    {

        public Movie Movie { get; set; }

        public List<UserComment> UserComments { get; set; }

        public List<UserRate> UserRates { get; set; }

    }
}
