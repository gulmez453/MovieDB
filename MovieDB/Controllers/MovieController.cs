using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using System.Security.Claims;

namespace MovieDB.Controllers
{
    public class MovieController : Controller
    {

        private readonly DatabaseContext _databaseContext;
        public MovieController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        private List<MovieComments> getMovieComments(List<Movie> allMovies)
        {
            List<MovieComments> commentedMovies = new List<MovieComments>();
            foreach (Movie movie in allMovies)
            {
                List<UserComment> userComments = new List<UserComment>();
                List<MovieUser> movieUserComments = _databaseContext.MoviesUsers.Where(mu => mu.MovieId == movie.Id).ToList();

                foreach (MovieUser movieUser in movieUserComments)
                {
                    User user = _databaseContext.Users.SingleOrDefault(user => user.Id == movieUser.UserId);
                    Comment comment = _databaseContext.Comments.SingleOrDefault(comment => comment.MovieUserId == movieUser.Id);
                    UserComment userComment = new UserComment
                    {
                        User = user,
                        Comment = comment
                    };

                    userComments.Add(userComment);
                }

                MovieComments movieComments = new MovieComments
                {
                    Movie = movie,
                    UserComments = userComments
                };
                commentedMovies.Add(movieComments);

            }
            return commentedMovies;
        }

        public IActionResult Index()
        {
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            return View(getMovieComments(allMovies));
        }

        [HttpPost]
        public IActionResult AddComment(Guid movieId, string commentText)
        {
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            MovieUser movieUser = new MovieUser
            {
                MovieId = movieId,
                UserId = userid,
            };

            _databaseContext.Add(movieUser);
            _databaseContext.SaveChanges();

            Comment comment = new Comment
            {
                MovieUserId = movieUser.Id,
                CommentText = commentText,
                CreatedAt = DateTime.Now,
            };

            _databaseContext.Add(comment);
            _databaseContext.SaveChanges(); 

            return RedirectToAction("Index");
        }

        public IActionResult RemoveComment(Guid commentId)
        {
            Comment comment;
            comment = _databaseContext.Comments.SingleOrDefault(comment => comment.Id == commentId);
            if(comment != null)
            {
                Guid movieUserId = comment.MovieUserId;
                _databaseContext.Remove(comment);

                MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.Id == movieUserId);
                if (movieUser != null)
                {
                    _databaseContext.Remove(movieUser);
                }
            }

            _databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult SearchMovie()
        {
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult SearchMovie(string searchText)
        {
            List<Movie> allMovies;
            if (searchText != null && searchText != "")
                allMovies = _databaseContext.Movies.Where(movie => movie.Title.Contains(searchText.Trim())).ToList();

            else
                allMovies = _databaseContext.Movies.ToList();
            return View("Index", getMovieComments(allMovies));

        }

        public IActionResult GetImage(Guid movieId)
        {
            Movie movie = _databaseContext.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie != null && movie.Image != null)
            {
                return File(movie.Image, "image/jpeg"); // Modify the content type based on your image format
            }

            // If the movie or image is not found, you can return a default image or an error message
            return File("~/images/default.jpg", "image/jpeg"); // Replace with your default image path and content type
        }
    }
}
