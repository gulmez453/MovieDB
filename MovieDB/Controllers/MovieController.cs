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

        public IActionResult Index()
        {
            List<MovieComments> commentedMovies = new List<MovieComments>();
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            
            foreach(Movie movie in allMovies)
            {
                List<UserComment> userComments = new List<UserComment>();
                List<MovieUser> movieUserComments = _databaseContext.MovieUserComments.Where(muc => muc.MovieId == movie.Id).ToList();
            
                foreach(MovieUser movieUser in movieUserComments)
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

            return View(commentedMovies);
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
