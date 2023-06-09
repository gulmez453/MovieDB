using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieDB.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //[Authorize]
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
