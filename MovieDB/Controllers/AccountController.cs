using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Entities;
using MovieDB.Models;
using NETCore.Encrypt.Extensions;
using System.Security.Claims;

namespace MovieDB.Controllers
{

    public class AccountController : Controller

    {

        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configiration;


        public AccountController(DatabaseContext databaseContext, IConfiguration configiration)
        {
            _databaseContext = databaseContext;
            _configiration = configiration;
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
                string Salt = _configiration.GetValue<string>("AppSettings:Salt");
                string saltedpass = model.Password + Salt;
                string hashedPasword = saltedpass.MD5();

                User user = _databaseContext.Users.SingleOrDefault(x => x.email.ToLower() == model.email.ToLower() && x.Password == hashedPasword);
                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim("Id", user.Id.ToString()));
                    claims.Add(new Claim("Name", user.FullName));
                    claims.Add(new Claim("email", user.email));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Username or password is incorrect.");
                }
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
                if (_databaseContext.Users.Any(x => x.email.ToLower()== model.email.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.email), "Email is already exists.");
                    return View(model);
                }
                string Salt = _configiration.GetValue<string>("AppSettings:Salt");
                string saltedpass = model.Password + Salt;
                string hashedPasword = saltedpass.MD5();

                User user = new ()
                {
                    email = model.email,
                    Password = hashedPasword,
                    FullName = model.FullName
                };

                _databaseContext.Users.Add(user);
                
                int affectedRowCount = _databaseContext.SaveChanges();

                if(affectedRowCount == 0)
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
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
