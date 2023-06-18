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
    [Authorize]  // Unless otherwise specified, it requires login.
    public class AccountController : Controller  //miras

    {
        
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configiration;
        //To inject dependencies  for AccountController,
        //we will use DatabaseContext and IConfiguration.

        public AccountController(DatabaseContext databaseContext, IConfiguration configiration)
        {
            _databaseContext = databaseContext;
            _configiration = configiration;
        }
        [AllowAnonymous] // display login
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost] // login post action
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                // for uniqe password hash we use salt function
                string hashedPasword = SaltFunction(model.Password);

                User user = _databaseContext.Users.SingleOrDefault(x => x.email.ToLower() == model.email.ToLower() && x.Password == hashedPasword);
                if (user != null)
                {   // store data in claim list
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.FullName?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role?? string.Empty));
                    claims.Add(new Claim("email", user.email));
                    claims.Add(new Claim("id", user.Id.ToString()));
                    // authentication  coockie
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    // give ıdetinty
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    //method of login with principal
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {   // error mesage
                    ModelState.AddModelError("", "Username or password is incorrect.");
                }
            }

            return View(model);
        }

        [AllowAnonymous] // open register form
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]  // all visitor can reach 
        [HttpPost]  //register post method
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
                //create object  and add database
                User user = new()
                {
                    email = model.email,
                    Password = hashedPasword,
                    FullName = model.FullName
                };

                _databaseContext.Users.Add(user);
                // check if changes have been made
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
        // we use for salt fonktion NETCore.Encrypt API
        private string SaltFunction(string password)
        {   // we have additinal string in appsetting. we add salt and hash
            string Salt = _configiration.GetValue<string>("AppSettings:Salt");
            string saltedpass = password + Salt;
            string hashedPasword = saltedpass.MD5();
            return hashedPasword;
        }
        // open profil and fill inputs with profil loader
        public IActionResult Profile()
        {
            ProfileLoader();

            return View();
        }

        //for fill inputs after save 
        private void ProfileLoader()
        {
            Guid userid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(a => a.Id == userid);

            ViewData["email"] = user.email;
            ViewData["FullName"] = user.FullName;
        }

        [HttpPost] // for profil  email change post data
        public IActionResult ProfileChangeEmail([Required][StringLength(30)] string? email)
        {
            if(ModelState.IsValid)
            {
                Guid userid = new Guid (User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault ( a => a.Id == userid);

                user.email = email;
                _databaseContext.SaveChanges(); // save chages
                ViewData["result"] = "EmailChanged"; // give message
              
                
            }
            ProfileLoader(); // get updated data
            return View("Profile");
        }

        [HttpPost]//for profil email change post data
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

        [HttpPost] // for change password
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
        // for using logout
        public IActionResult Logout() //log out and redirect index
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
