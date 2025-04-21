using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AdministrationDataBase.Context;
using AdministrationDataBase.Helpers;
using AdministrationDataBaseData.Models;
using System.Security.Claims;
using AdministrationDataBase.Resources;

namespace AdministrationDataBase.Controllers
{
    public class AccountController : Controller
    {
        private readonly BDContext _db;
        private readonly IConfiguration _conf;
        private readonly IWebHostEnvironment _env;
        private const int SessionDurationHours = 12;

        public AccountController(BDContext db, IWebHostEnvironment env, IConfiguration conf)
        {
            _db = db;
            _env = env;
            _conf = conf;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return User.Identity?.IsAuthenticated ?? false
                ? RedirectToAction("Index", "Home")
                : View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "MassagesCustomer");

            if (string.IsNullOrEmpty(user.Email))
                return View(user);
            
            var passwordHash = HashHelper.GetMD5Hash(user.Password ?? string.Empty);
            var validUser = UserHelper.GetUserByEmailAndPassword(_db, user.Email, passwordHash);

            if (validUser is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View(user);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, validUser.Email),
                new(ClaimTypes.NameIdentifier, validUser.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(SessionDurationHours)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "MassagesCustomer");
        }

        [HttpGet] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost() 
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string hash)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Inscripcion");

            if (string.IsNullOrEmpty(hash))
                return RedirectToAction("Login");

            var (user, error) = ValidatePasswordResetRequest(hash);
            ViewBag.Error = error;

            return user != null ? View(user) : RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string hash, string password, string passwordRepeat)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");

            var (user, error) = ValidatePasswordResetRequest(hash);
            var errorPassword = ValidatePasswords(password, passwordRepeat);

            if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(errorPassword))
            {
                ViewBag.Error = error;
                ViewBag.ErrorPassword = errorPassword;
                return RedirectToAction("ChangePassword", new { hash });
            }

            user.Password = HashHelper.GetMD5Hash(password);
            user.Hash = null;
            user.HashDate = null;

            _db.Update(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }
        private (User user, string error) ValidatePasswordResetRequest(string hash)
        {
            var user = _db.Users.FirstOrDefault(x => x.Hash == hash);
            if (user == null)
                return (null, Resource.ActivateAccountErrorDoesntExist);

            if (!DateHelper.IsInTime(user.HashDate.GetValueOrDefault()))
                return (null, Resource.ActivateAccountErrorTimeLimitExceeded);

            return (user, null);
        }

        private static string ValidatePasswords(string password, string passwordRepeat)
        {
            return string.IsNullOrEmpty(password) || password != passwordRepeat
                ? Resource.PasswordsDoNotMatch
                : string.Empty;
        }
    }
}
