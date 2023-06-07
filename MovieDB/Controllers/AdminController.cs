using Microsoft.AspNetCore.Mvc;

namespace MovieDB.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public  IActionResult AddMovie()
        {
            return View();
        }
    }
}
