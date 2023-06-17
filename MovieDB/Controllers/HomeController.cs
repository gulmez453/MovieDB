using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using System.Diagnostics;

namespace MovieDB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _databaseContext;

        public HomeController(ILogger<HomeController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
        
        }
        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            List<Movie> allMovies = _databaseContext.Movies.ToList();

            MovieFilterViewModel movieFilterViewModel = new MovieFilterViewModel { MovieViewModel = allMovies };

            return View(movieFilterViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(string filterSearchText, string Category, string ProduceYearMin,
            string ProduceYearMax, string MinuteMin, string MinuteMax, bool Rate1, bool Rate2,
            bool Rate3, bool Rate4, bool Rate5)
        {
            
            List<Movie> allMovies;
            if (filterSearchText != null && filterSearchText != "")
                allMovies = _databaseContext.Movies.Where(movie => movie.Title.Contains(filterSearchText.Trim())).ToList();
            else
                allMovies = _databaseContext.Movies.ToList();

            MovieFilterViewModel movieFilterViewModel = new MovieFilterViewModel { MovieViewModel = allMovies };

            return View(movieFilterViewModel);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}