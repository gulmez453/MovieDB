using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using NETCore.Encrypt.Extensions;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;
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

        public IActionResult AddMovie()
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

        [HttpPost]
        [ActionName("AddMovie")]
        public  IActionResult AddMovie(MovieViewModel model)
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
                    Producer = model.Producer,
                    Director = model.Director,
                    MusicDirector = model.MusicDirector,
                    ProduceIn = model.ProduceIn,
                    Image = imageData,
                };

                _databaseContext.Movies.Add(movie);

                int affectedRowCount = _databaseContext.SaveChanges();

                if (affectedRowCount == 0)
                {
                    ModelState.AddModelError("", "The movie can not be added");
                }

            }

            return View(model);
        }

        [HttpPost]
        [ActionName("UpdateMovie")]
        public  IActionResult UpdateMovie(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the existing movie by its unique identifier, such as ID
                Movie existingMovie = _databaseContext.Movies.FirstOrDefault(movie => movie.Title == model.Title);

                if (existingMovie != null)
                {
                    // Update the properties of the existing movie with the new values
                    existingMovie.Title = model.Title;
                    existingMovie.Producer = model.Producer;
                    existingMovie.Director = model.Director;
                    existingMovie.MusicDirector = model.MusicDirector;
                    existingMovie.ProduceIn = model.ProduceIn;

                    // Save the changes to the database
                    _databaseContext.SaveChanges();

                    return RedirectToAction("Index", "Admin"); // Redirect to the Admin Index page after successful update
                }
                else
                {
                    ModelState.AddModelError("", "The movie does not exist");
                }
            }


            return View(model);
        }


        [HttpPost]
        [ActionName("RemoveMovie")]
        public IActionResult RemoveMovie(Guid userId)
        {
            Movie movie = _databaseContext.Movies.FirstOrDefault(movie => movie.Id == userId);

            if (movie != null)
            {
                _databaseContext.Movies.Remove(movie);

                int affectedRowCount = _databaseContext.SaveChanges();

                if (affectedRowCount == 0)
                {
                    ModelState.AddModelError("", "The movie can not be removed");
                }

            }

            return View("ListMovies");
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
        public IActionResult UpdateMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateProfileEmail([Required][StringLength(30)] string? email)
        {

            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            user.email = email;
            _databaseContext.SaveChanges();
            ViewData["result"] = "EmailChanged";


            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfileName([Required][StringLength(30)] string? name)
        {

            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            user.FullName = name;
            _databaseContext.SaveChanges();
            ViewData["result"] = "NameChanged";


         
            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfilePassword([Required][MinLength(6)][MaxLength(16)] string? password)
        {

            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            string hashedPassword = SaltFunction(password);

            user.Password = hashedPassword;
            _databaseContext.SaveChanges();

            ViewData["result"] = "PasswordChanged";
            
       
            return View("UpdateUser", user);
        }

        [HttpPost]
        public IActionResult UpdateProfileRole([Required] string role)
        {

            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            user.Role = role == "on" ? "admin" : "user";
            _databaseContext.SaveChanges();
            ViewData["result"] = "RoleChanged";



            return View("UpdateUser", user);
        }

    }
}
