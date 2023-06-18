using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Entities;
using MovieDB.Models;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;

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

                // get the movieUsers
                List<MovieUser> movieUsers = _databaseContext.MoviesUsers.Where(mu => mu.MovieId == movie.Id).ToList();
                

                foreach (MovieUser movieUser in movieUsers)
                {
                    // get the comments and rates releated with movieUser
                    List<Comment> comments = _databaseContext.Comments.Where(comment => comment.MovieUserId == movieUser.Id).ToList();
                    List<Rate> rates = _databaseContext.Rates.Where(rate => rate.MovieUserId == movieUser.Id).ToList();
                    User user = _databaseContext.Users.SingleOrDefault(user => user.Id == movieUser.UserId);

                    foreach (Comment comment in comments)
                    {
                        // create a userComment model
                        UserComment userComment = new UserComment
                        {
                            User = user,
                            Comment = comment
                        };

                        // add the userComment to the list
                        userComments.Add(userComment);
                    }

                    foreach (Rate rate in rates)
                    {
                        // create a userRate model
                        UserRate userRate = new UserRate
                        {
                            User = user,
                            Rate = rate
                        };

                        // add the userRate to the list
                        userRates.Add(userRate);
                    }


                }
  
                // return the lists to the sorted lists
                List<UserComment>  sortedUserComments = userComments.OrderBy(userComment => userComment.Comment.CreatedAt).ToList();
                List<UserRate> sortedUserRates = userRates.OrderBy(userRate=> userRate.Rate.CreatedAt).ToList();

                //create a MovieComments model
                MovieComments movieComments = new MovieComments
                {
                    Movie = movie,
                    UserComments = sortedUserComments,
                    UserRates = sortedUserRates
                };

                // add the movieComments to the list
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

        public IActionResult Index()
        {
            // list the movies
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            return View(getMovieComments(allMovies));
        }
        [Authorize]
        public IActionResult AddRate(int movieId, string[] rating)
        {
            int rateNum = rating.Length;

            // get the user
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // get the movieUser
            MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.UserId == userid && mu.MovieId == movieId);
            if(movieUser == null)
            {
                // create the movieser if it doesn't exist
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
                // create a rate
                rate = new Rate
                {
                    MovieUserId = movieUser.Id,
                    RateNum = rateNum,
                    CreatedAt = DateTime.Now,
                };

                // add the rate to the db
                _databaseContext.Add(rate);
                _databaseContext.SaveChanges();
            }
            else 
            {
                // set the rateNum if the rate exists
                rate.RateNum = rateNum;
                _databaseContext.SaveChanges();
            }

            string referer = HttpContext.Request.Headers["Referer"].ToString();
            if (referer.Contains ("Movie/Details"))            {
                return RedirectToAction("Details", new { movieId = movieId });
            }
            else 
            {
                return RedirectToAction("Index"); 
            }
        }
        
        [HttpPost]
        public IActionResult AddComment(int movieId, string commentText)
        {
            // get the user
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            // get the movieUser
            MovieUser movieUser = _databaseContext.MoviesUsers.SingleOrDefault(mu => mu.UserId == userid && mu.MovieId == movieId);
            if(movieUser == null) 
            {
                // create the movieser if it doesn't exist
                movieUser = new MovieUser
                {
                    MovieId = movieId,
                    UserId = userid,
                };
                _databaseContext.Add(movieUser);
                _databaseContext.SaveChanges();

            }

            // create a comment
            Comment comment = new Comment
            {
                MovieUserId = movieUser.Id,
                CommentText = commentText,
                CreatedAt = DateTime.Now,
            };

            // add the comment to the db
            _databaseContext.Add(comment);
            _databaseContext.SaveChanges();

            string referer = HttpContext.Request.Headers["Referer"].ToString();
            if (referer.Contains("Movie/Details"))
            {
                return RedirectToAction("Details", new { movieId = movieId });
            }
            else
            {
                return RedirectToAction("Index");
            }


        }


        public IActionResult RemoveComment(int commentId, int movieId)
        {
            // get the comment
            Comment comment;
            comment = _databaseContext.Comments.SingleOrDefault(comment => comment.Id == commentId);
            if (comment != null)
            {
                // remove the comment if it exists
                int movieUserId = comment.MovieUserId;
                _databaseContext.Remove(comment);

            }

            _databaseContext.SaveChanges();

            string referer = HttpContext.Request.Headers["Referer"].ToString();
            if (referer.Contains("Movie/Details"))
            {
                return RedirectToAction("Details", new { movieId = movieId });
            }
            else
            {
                return RedirectToAction("Index");
            };
            
        }

        public IActionResult SearchMovie()
        {
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult SearchMovie(string searchText)
        {
            // get the filtered movie list
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

            // return the image wiht byte array
            if (movie != null && movie.Image != null)
            {
                return File(movie.Image, "image/jpeg"); 
            }

            // return the default image it doesn't exist
            return File("~/images/default.jpg", "image/jpeg");
        }
    }
}
