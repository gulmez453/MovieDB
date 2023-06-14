using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using MovieDB.Entities;
using MovieDB.Models;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MovieDB.Controllers
{
    [Authorize]
    public class AccountController : Controller

    {
        
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configiration;


        public AccountController(DatabaseContext databaseContext, IConfiguration configiration)
        {
            _databaseContext = databaseContext;
            _configiration = configiration;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                
                string hashedPasword = SaltFunction(model.Password);

                User user = _databaseContext.Users.SingleOrDefault(x => x.email.ToLower() == model.email.ToLower() && x.Password == hashedPasword);
                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.FullName?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role?? string.Empty));
                    claims.Add(new Claim("email", user.email));
                    claims.Add(new Claim("id", user.Id.ToString()));

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

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.email.ToLower() == model.email.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.email), "Email is already exists.");
                    return View(model);
                }
                string hashedPasword = SaltFunction(model.Password);

                User user = new()
                {
                    email = model.email,
                    Password = hashedPasword,
                    FullName = model.FullName
                };

                _databaseContext.Users.Add(user);

                int affectedRowCount = _databaseContext.SaveChanges();

                if (affectedRowCount == 0)
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

        private string SaltFunction(string password)
        {
            string Salt = _configiration.GetValue<string>("AppSettings:Salt");
            string saltedpass = password + Salt;
            string hashedPasword = saltedpass.MD5();
            return hashedPasword;
        }

        public IActionResult Profile()
        {
            ProfileLoader();

            return View();
        }

        private void ProfileLoader()
        {
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            ViewData["email"] = user.email;
            ViewData["FullName"] = user.FullName;
        }

        [HttpPost]
        public IActionResult ProfileChangeEmail([Required][StringLength(30)] string? email)
        {
            if(ModelState.IsValid)
            {
                Guid userid = new Guid (User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault ( a => a.Id == userid);

                user.email = email;
                _databaseContext.SaveChanges();
                ViewData["result"] = "EmailChanged";
              
                
            }
            ProfileLoader();
            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangeName([Required][StringLength(30)] string? name)
        {
            if (ModelState.IsValid)
            {
                Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

                user.FullName = name;
                _databaseContext.SaveChanges();
                ViewData["result"] = "NameChanged";
                

            }
            ProfileLoader();
            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangePassword([Required][MinLength(6)][MaxLength(16)] string? password)
        {
            if (ModelState.IsValid)
            {
                Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

                string hashedPassword = SaltFunction(password);

                user.Password = hashedPassword;
                _databaseContext.SaveChanges();

                ViewData["result"] = "PasswordChanged";
            }
            ProfileLoader();
            return View("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
