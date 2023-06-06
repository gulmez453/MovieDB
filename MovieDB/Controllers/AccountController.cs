using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;

namespace MovieDB.Controllers
{

    public class AccountController : Controller

    {

        private readonly DatabaseContext _databaseContext;

        public AccountController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

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
                User user = new User()
                {
                    email = model.email,
                    Password = model.Password
                };

                _databaseContext.Users.Add(user);
                
                int affectedRowCount = _databaseContext.SaveChanges();

                if(affectedRowCount > 0)
                {
                    ModelState.AddModelError("", "User can not be added");
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }

            }

            return View(model);
           
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
