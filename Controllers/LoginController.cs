using EMSProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMSProject.Controllers
{
    public class LoginController : Controller
    {
        private const string _username = "KARTHIKEIN";
        private const string _password = "KUSHIKARTHIK";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginModel model)
        {
            if (model.Username == _username && model.Password == _password)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Employee");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
