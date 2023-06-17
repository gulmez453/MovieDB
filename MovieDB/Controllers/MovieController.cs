using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Entities;
using MovieDB.Models;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
                List<UserRate> userRates = new List<UserRate>();
                List<MovieUser> movieUsers = _databaseContext.MoviesUsers.Where(mu => mu.MovieId == movie.Id).ToList();
                

                foreach (MovieUser movieUser in movieUsers)
                {
                    List<Comment> comments = _databaseContext.Comments.Where(comment => comment.MovieUserId == movieUser.Id).ToList();
                    List<Rate> rates = _databaseContext.Rates.Where(rate => rate.MovieUserId == movieUser.Id).ToList();
                    User user = _databaseContext.Users.SingleOrDefault(user => user.Id == movieUser.UserId);

                    foreach (Comment comment in comments)
                    {
                        UserComment userComment = new UserComment
                        {
                            User = user,
                            Comment = comment
                        };

                        userComments.Add(userComment);
                    }

                    foreach (Rate rate in rates)
                    {
                        UserRate userRate = new UserRate
                        {
                            User = user,
                            Rate = rate
                        };

                        userRates.Add(userRate);
                    }


                }
  

                List<UserComment>  sortedUserComments = userComments.OrderBy(userComment => userComment.Comment.CreatedAt).ToList();
                List<UserRate> sortedUserRates = userRates.OrderBy(userRate=> userRate.Rate.CreatedAt).ToList();

                MovieComments movieComments = new MovieComments
                {
                    Movie = movie,
                    UserComments = sortedUserComments,
                    UserRates = sortedUserRates
                };

                commentedMovies.Add(movieComments);

            }
            return commentedMovies;
        }
        public IActionResult Details(int movieId)
        {
            var movie = _databaseContext.Movies.SingleOrDefault(m => m.Id == movieId);
            if (movie == null)
            {
                return NotFound();
            }

            var movieCommentsModel = getMovieComments(new List<Movie> { movie }).FirstOrDefault();

            return View(movieCommentsModel);
        }



        /*
        public IActionResult Details(Guid movieId)
        {
            Movie movie = _databaseContext.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie == null)
            {
                return NotFound(); // Return a 404 Not Found error if the movie is not found
            }

            return View(movie);
        }
        */
        public IActionResult Index()
        {
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            return View(getMovieComments(allMovies));
        }
        [Authorize]
        public IActionResult AddRate(int movieId, string[] rating)
        {
            int rateNum = rating.Length;
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.UserId == userid && mu.MovieId == movieId);
            if(movieUser == null)
            {
                movieUser = new MovieUser
                {
                    MovieId = movieId,
                    UserId = userid,
                };
                _databaseContext.Add(movieUser);
                _databaseContext.SaveChanges();
            }

            Rate rate = _databaseContext.Rates.SingleOrDefault(mu => mu.MovieUserId == movieUser.Id);
            if (rate == null)
            {

                rate = new Rate
                {
                    MovieUserId = movieUser.Id,
                    RateNum = rateNum,
                    CreatedAt = DateTime.Now,
                };

                _databaseContext.Add(rate);
                _databaseContext.SaveChanges();
            }
            else 
            {
                rate.RateNum = rateNum;
                _databaseContext.SaveChanges();
            }


          

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult AddComment(int movieId, string commentText)
        {
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.UserId == userid && mu.MovieId == movieId);
            if(movieUser == null) 
            {
                movieUser = new MovieUser
                {
                    MovieId = movieId,
                    UserId = userid,
                };
                _databaseContext.Add(movieUser);
                _databaseContext.SaveChanges();

            }


            Comment comment = new Comment
            {
                MovieUserId = movieUser.Id,
                CommentText = commentText,
                CreatedAt = DateTime.Now,
            };

            _databaseContext.Add(comment);
            _databaseContext.SaveChanges();

            return RedirectToAction("Details", new { movieId = movieId });

            
        }

        public IActionResult RemoveComment(int commentId)
        {
            Comment comment;
            comment = _databaseContext.Comments.SingleOrDefault(comment => comment.Id == commentId);
            if (comment != null)
            {
                int movieUserId = comment.MovieUserId;
                _databaseContext.Remove(comment);
                /*
                MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.Id == movieUserId);
                if (movieUser != null)
                {
                    _databaseContext.Remove(movieUser);
                }
                */
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

        public IActionResult GetImage(int movieId)
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
