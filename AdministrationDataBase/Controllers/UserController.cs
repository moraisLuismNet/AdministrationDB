using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdministrationDataBase.Context;
using AdministrationDataBase.Helpers;
using AdministrationDataBaseData.Models;

namespace AdministrationDataBase.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly BDContext _db;
        private readonly IConfiguration _conf;

        public UserController(BDContext db, IConfiguration conf)
        {
            _db = db;
            _conf = conf;
        }

        public IActionResult Index()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);
            List<User> users = _db.Users.ToList();

            return View(users);
        }

        public IActionResult CreateUser()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(IFormCollection form)
        {
            try
            {
                string mail = form["email"];
                User user = _db.Users.Where(u => u.Email == mail).FirstOrDefault();

                if (user != null)
                {
                    return BadRequest("The mail is already in use");
                }

                string password = form["password"];
                string passwordRepeat = form["passwordRepeat"];
                if (password != passwordRepeat)
                {
                    return BadRequest("The passwords do not match");
                }

                // Calculate the password hash
                string passwordHash = HashHelper.GetMD5Hash(password);

                // Create the new user with the calculated hash
                user = new User
                {
                    Name = form["name"],
                    Email = mail,
                    Password = passwordHash
                };

                _db.Users.Add(user);
                _db.SaveChanges();

                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        public IActionResult EditUser(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            User user = _db.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(IFormCollection form)
        {
            User user = _db.Users.Find(int.Parse(form["id"]));

            user.Name = form["name"];
            user.Email = form["email"];

            if (!string.IsNullOrEmpty(form["password"]) && !string.IsNullOrEmpty(form["passwordRepeat"]) && form["password"] == form["passwordRepeat"])
            {
                user.Password = HashHelper.GetMD5Hash(form["password"]);
            }

            _db.Users.Update(user);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteUser(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            User user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                User user = _db.Users.Find(id);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                _db.Users.Remove(user);
                _db.SaveChanges();

                return Ok(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}