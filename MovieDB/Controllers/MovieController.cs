using Microsoft.AspNetCore.Mvc;

namespace MovieDB.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
