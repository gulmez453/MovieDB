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
        private List<string> AllCategories;
        public HomeController(ILogger<HomeController> logger, DatabaseContext databaseContext)
        {
            _logger = logger;
            _databaseContext = databaseContext;
            AllCategories = this.GetCategories();
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
            FilterViewModel filterViewModel = new() { AllCategories = this.AllCategories, Rates = new List<bool> { true, true, true, true, true } };
            MovieFilterViewModel movieFilterViewModel = new() { MovieViewModel = allMovies , FilterViewModel=filterViewModel};

            return View(movieFilterViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(
            string filterSearchText, string Category, string ProduceYearMin,string ProduceYearMax,
            string MinuteMin, string MinuteMax, string[] Rate)
        {
            // Preparing Rates
            List<bool> Rates = new() { false, false, false, false, false};
            foreach(string r in Rate)
            {
                Rates[int.Parse(r)-1] = true;
            }

            FilterViewModel filterViewModel = new()
            {
                Search = filterSearchText,
                Category = Category,
                ProduceYearMin = ProduceYearMin,
                ProduceYearMax = ProduceYearMax,
                MinuteMin = MinuteMin,
                MinuteMax = MinuteMax,
                Rates = Rates,
                AllCategories = this.AllCategories
            };

            List<Movie> filteredMovies = this.FilterMovies(filterViewModel);

            MovieFilterViewModel movieFilterViewModel = new()
            {
                MovieViewModel = filteredMovies ,
                FilterViewModel = filterViewModel
            };

            return View(movieFilterViewModel);
        }

        private List<Movie> FilterMovies(FilterViewModel filterViewModel)
        {
            IQueryable<Movie> x = _databaseContext.Movies.AsQueryable();

            if (filterViewModel.Search != null && filterViewModel.Search != "")
                x = x.Where(movie => movie.Title.Contains(filterViewModel.Search.Trim()));
            if (filterViewModel.Category != "All Categories")
                x = x.Where(movie => movie.Type.Equals(filterViewModel.Category));
            if (filterViewModel.ProduceYearMin != null && filterViewModel.ProduceYearMin != "")
                x = x.Where(movie => movie.ProduceYear >= int.Parse(filterViewModel.ProduceYearMin));
            if (filterViewModel.ProduceYearMax != null && filterViewModel.ProduceYearMax != "")
                x = x.Where(movie => movie.ProduceYear <= int.Parse(filterViewModel.ProduceYearMax));
            if (filterViewModel.MinuteMin != null && filterViewModel.MinuteMin != "")
                x = x.Where(movie => movie.Hour * 60 + movie.Minute >= int.Parse(filterViewModel.MinuteMin));
            if (filterViewModel.MinuteMax != null && filterViewModel.MinuteMax != "")
                x = x.Where(movie => movie.Hour * 60 + movie.Minute <= int.Parse(filterViewModel.MinuteMax));
            x = x.Where(movie =>
            (filterViewModel.Rates[0] && movie.Rate == 1) ||
            (filterViewModel.Rates[1] && movie.Rate == 2) ||
            (filterViewModel.Rates[2] && movie.Rate == 3) ||
            (filterViewModel.Rates[3] && movie.Rate == 4) ||
            (filterViewModel.Rates[4] && movie.Rate == 5)) ;

            return x.ToList();
        }

        private List<string> GetCategories()
        {
            return _databaseContext.Movies.Select(movie => movie.Type).Distinct().ToList();
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