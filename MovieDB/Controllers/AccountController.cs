using Microsoft.AspNetCore.Mvc;
using MovieDB.Models;

namespace MovieDB.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                // login islemleri
            }

            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // register islemleri
            }

            return View(model);
           
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
