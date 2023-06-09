using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using NETCore.Encrypt.Extensions;
using NuGet.Protocol.Plugins;

namespace MovieDB.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        public AdminController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddMovie()
        {
            return View();
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

                Movie movie = new Movie()
                {
                    Title = model.Title,
                    Producer = model.Producer,
                    Director = model.Director,
                    MusicDirector = model.MusicDirector,
                    ProduceIn = model.ProduceIn,
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
        public IActionResult UpdateMovie()
        {
            return View();
        }
    }
}
