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
            AllCategories = this.GetCategories(); // This will calculate only one time, and it will be used for filter side category selectbox
        }
        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpGet]
       
        


        public IActionResult Index()
        {
            // This is default view method
            // getting all movies
            // then creates a FilterViewModel with allCategories and defaultly all rates are checked
            List<Movie> allMovies = _databaseContext.Movies.ToList();
            FilterViewModel filterViewModel = new() { AllCategories = this.AllCategories, Rates = new List<bool> { true, true, true, true, true } };
            MovieFilterViewModel movieFilterViewModel = new() { MovieViewModel = allMovies , FilterViewModel=filterViewModel};


            return View(movieFilterViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(
            string filterSearchText, string actorSearch, string directorSearch, string Category,
            string ProduceYearMin, string ProduceYearMax,string MinuteMin, string MinuteMax, string[] Rate, int pageNumber = 1)
        {
            // Preparing Rates for understant which indexes are checked
            List<bool> Rates = new() { false, false, false, false, false };
            foreach (string r in Rate)
            {
                Rates[int.Parse(r) - 1] = true;
            }

            FilterViewModel filterViewModel = new()
            {
                Search = filterSearchText,
                ActorSearch = actorSearch,
                DirectorSearch = directorSearch,
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
                MovieViewModel = filteredMovies,
                FilterViewModel = filterViewModel,
                
            };

            return View(movieFilterViewModel);
        }

        private List<Movie> FilterMovies(FilterViewModel filterViewModel)
        {
            // To be able to send a query as a chain I get all movies as IQueryable
            IQueryable<Movie> x = _databaseContext.Movies.AsQueryable();

            // If users entered something to movie search bar, filter it
            if (filterViewModel.Search != null && filterViewModel.Search != "")
                x = x.Where(movie => movie.Title.Contains(filterViewModel.Search.Trim()));

            // If users entered something to actor search bar, filter it
            if (filterViewModel.ActorSearch != null && filterViewModel.ActorSearch != "")
                x = x.Where(movie => movie.Artists.Contains(filterViewModel.ActorSearch.Trim()));

            // If users entered something to director search bar, filter it
            if (filterViewModel.DirectorSearch != null && filterViewModel.DirectorSearch != "")
                x = x.Where(movie => movie.Director.Contains(filterViewModel.DirectorSearch.Trim()));

            // If users selected a category, filter it
            if (filterViewModel.Category != "All Categories")
                x = x.Where(movie => movie.Type.Equals(filterViewModel.Category));

            // If users entered something to produceyearmin search bar, filter it
            if (filterViewModel.ProduceYearMin != null && filterViewModel.ProduceYearMin != "")
                x = x.Where(movie => movie.ProduceYear >= int.Parse(filterViewModel.ProduceYearMin));

            // If users entered something to produceyearmax search bar, filter it
            if (filterViewModel.ProduceYearMax != null && filterViewModel.ProduceYearMax != "")
                x = x.Where(movie => movie.ProduceYear <= int.Parse(filterViewModel.ProduceYearMax));

            // If users entered something to minutemin search bar, filter it
            if (filterViewModel.MinuteMin != null && filterViewModel.MinuteMin != "")
                x = x.Where(movie => movie.Hour * 60 + movie.Minute >= int.Parse(filterViewModel.MinuteMin));

            // If users entered something to minutemax search bar, filter it
            if (filterViewModel.MinuteMax != null && filterViewModel.MinuteMax != "")
                x = x.Where(movie => movie.Hour * 60 + movie.Minute <= int.Parse(filterViewModel.MinuteMax));

            // Checksboxes are controlling and necessary ones are usşng for filtering
            x = x.Where(movie =>
            (filterViewModel.Rates[0] && movie.Rate == 1) ||
            (filterViewModel.Rates[1] && movie.Rate == 2) ||
            (filterViewModel.Rates[2] && movie.Rate == 3) ||
            (filterViewModel.Rates[3] && movie.Rate == 4) ||
            (filterViewModel.Rates[4] && movie.Rate == 5));

            return x.ToList(); // converting IQueryable to List
        }

        private List<string> GetCategories()
        {
            // Getting all unique category names from database
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

     
    public IActionResult Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Process the contact form submission (e.g., send an email)

            // Set a success message to be displayed on the page
            ViewBag.Message = "Your message has been sent successfully. We will get back to you soon!";
        }

        return View(model);
    }
    }
}