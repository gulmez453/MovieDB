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
        public IActionResult Index(
            string filterSearchText, string Category, string ProduceYearMin,string ProduceYearMax,
            string MinuteMin, string MinuteMax, bool Rate1, bool Rate2, bool Rate3, bool Rate4, bool Rate5)
        {
            FilterViewModel filterViewModel = new FilterViewModel
            {
                Search = filterSearchText,
                Category = Category,
                ProduceYearMin = ProduceYearMin,
                ProduceYearMax = ProduceYearMax,
                MinuteMin = MinuteMin,
                MinuteMax = MinuteMax,
                Rate1 = Rate1,
                Rate2 = Rate2,
                Rate3 = Rate3,
                Rate4 = Rate4,
                Rate5 = Rate5
            };

            List<Movie> filteredMovies = this.FilterMovies(filterViewModel);

            MovieFilterViewModel movieFilterViewModel = new MovieFilterViewModel {
                MovieViewModel = filteredMovies ,
                FilterViewModell = filterViewModel
            };

            return View(movieFilterViewModel);
        }

        private List<Movie> FilterMovies(FilterViewModel filterViewModel)
        {
             List<Movie> allMovies;
            if (filterViewModel.Search != null && filterViewModel.Search != "")
                allMovies = _databaseContext.Movies.Where(movie => movie.Title.Contains(filterViewModel.Search.Trim())).ToList();
            else
                allMovies = _databaseContext.Movies.ToList();
            return allMovies;
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