using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using NETCore.Encrypt.Extensions;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using System.Xml.Linq;

namespace MovieDB.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configiration;
        public AdminController(DatabaseContext databaseContext, IConfiguration configiration)
        {
            _databaseContext = databaseContext;
            _configiration = configiration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListUsers()
        {
            List<User> allMovies = _databaseContext.Users.ToList();
            return View(allMovies);

        }
        public IActionResult ListMovies()
        {
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            return View(allMovies);
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

        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Movies.Any(movie => movie.Title.ToLower() == model.Title.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Title), "The movie is already exists.");
                    return View(model);
                }

                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    model.Image.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                Movie movie = new Movie()
                {
                    Title = model.Title,
                    Artists = model.Artists,
                    Director = model.Director,
                    Type = model.Type,
                    ProduceYear = model.ProduceYear,
                    Rate = model.Rate,
                    Hour = model.Hour,
                    Minute = model.Minute,
                    Description = model.Description,
                    Image = imageData,
                    fragman = model.fragman
                };

                _databaseContext.Movies.Add(movie);

                int affectedRowCount = _databaseContext.SaveChanges();

                if (affectedRowCount == 0)
                {
                    ModelState.AddModelError("", "The movie can not be added");

                }
                else
                {
                    return RedirectToAction("ListMovies");
                }

            }
            return View();
        }

        public IActionResult UpdateMovie(int movieId)
        {
            ViewData["movieId"] = movieId;
            return View();
        }

        [HttpPost]
        public  IActionResult UpdateMovie(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the existing movie by its unique identifier, such as ID
                Movie existingMovie = _databaseContext.Movies.FirstOrDefault(movie => movie.Id == model.Id);

                if (existingMovie != null)
                {
                    // Update the properties of the existing movie with the new values
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        model.Image.CopyTo(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    existingMovie.Title = model.Title;
                    existingMovie.Artists = model.Artists;
                    existingMovie.Director = model.Director;
                    existingMovie.Type = model.Type;
                    existingMovie.ProduceYear = model.ProduceYear;
                    existingMovie.Rate = model.Rate;
                    existingMovie.Hour = model.Hour;
                    existingMovie.Minute = model.Minute;
                    existingMovie.Description = model.Description;
                    existingMovie.Image = imageData;

                    // Save the changes to the database
                    _databaseContext.SaveChanges();

                    return RedirectToAction("ListMovies");
                }
                else
                {
                    ModelState.AddModelError("", "The movie does not exist");
                }
            }
            return View();

        }

        public IActionResult RemoveMovie(int movieId)
        {
            Movie movie = _databaseContext.Movies.FirstOrDefault(m => m.Id == movieId);

            if (movie != null)
            {
                List<MovieUser> movieUsers = _databaseContext.MoviesUsers.Where(mu => mu.MovieId == movieId).ToList();
                foreach (MovieUser movieUser in movieUsers)
                {
                    List<Comment> comments = _databaseContext.Comments.Where(comment => comment.MovieUserId == movieUser.Id).ToList();
                    _databaseContext.Comments.RemoveRange(comments);

                    Rate rate = _databaseContext.Rates.FirstOrDefault(rate => rate.MovieUserId == movieUser.Id);
                    if (rate != null)
                    {
                        _databaseContext.Rates.Remove(rate);
                    }
                }

                _databaseContext.MoviesUsers.RemoveRange(movieUsers);
                _databaseContext.Movies.Remove(movie);

                int result = _databaseContext.SaveChanges();

                if (result == 0)
                {
                    ModelState.AddModelError("", "The movie could not be removed.");
                }
            }

            return RedirectToAction("ListMovies");
        }

        public IActionResult RemoveUser(Guid userId)
        {
            User user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                List<MovieUser> movieUsers = _databaseContext.MoviesUsers.Where(mu => mu.UserId == userId).ToList();
                foreach (MovieUser movieUser in movieUsers)
                {
                    List<Comment> comments = _databaseContext.Comments.Where(comment => comment.MovieUserId == movieUser.Id).ToList();
                    _databaseContext.Comments.RemoveRange(comments);

                    Rate rate = _databaseContext.Rates.FirstOrDefault(rate => rate.MovieUserId == movieUser.Id);
                    if (rate != null)
                    {
                        _databaseContext.Rates.Remove(rate);
                    }
                }

                _databaseContext.MoviesUsers.RemoveRange(movieUsers);
                _databaseContext.Users.Remove(user);

                int result = _databaseContext.SaveChanges();
                if (result == 0)
                {
                    ModelState.AddModelError("", "The user could not be removed.");
                }

            }

            return RedirectToAction("ListUsers");
        }


        public IActionResult UpdateUser(Guid userId)
        {
            User user_filtered = _databaseContext.Users.SingleOrDefault(user => user.Id == userId);
            return View(user_filtered);
        }
        private string SaltFunction(string password)
        {
            string Salt = _configiration.GetValue<string>("AppSettings:Salt");
            string saltedpass = password + Salt;
            string hashedPasword = saltedpass.MD5();
            return hashedPasword;
        }
        
        [HttpPost]
        public IActionResult UpdateProfileEmail(Guid userId, [Required][StringLength(30)] string? email)
        {
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userId);

            user.email = email;
            _databaseContext.SaveChanges();
            ViewData["result"] = "EmailChanged";


            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfileName(Guid userId, [Required][StringLength(30)] string? name)
        {
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userId);

            user.FullName = name;
            _databaseContext.SaveChanges();
            ViewData["result"] = "NameChanged";


         
            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfilePassword(Guid userId, [Required][MinLength(6)][MaxLength(16)] string? password)
        {
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userId);

            string hashedPassword = SaltFunction(password);

            user.Password = hashedPassword;
            _databaseContext.SaveChanges();

            ViewData["result"] = "PasswordChanged";
            
       
            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfileRole(Guid userId, string? role)
        {
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userId);

            user.Role = role == "on" ? "admin" : "user";
            _databaseContext.SaveChanges();
            ViewData["result"] = "RoleChanged";



            return View("UpdateUser", user);
        }

    }
}
