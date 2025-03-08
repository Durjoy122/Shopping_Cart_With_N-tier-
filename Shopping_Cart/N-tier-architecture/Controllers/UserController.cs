using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DAL.Models;

namespace N_tire_architecture.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the same view to show the validation errors
                return View(user);
            }
            var isRegistered = _userService.RegisterUser(user);
            if (!isRegistered)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(user);
            }
            TempData["SuccessMessage"] = "Registration successful! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }


        // Login 
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                ViewData["Email"] = email; // Keep email in input
                return View();
            }

            var user = _userService.ValidateUser(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                ViewData["Email"] = email; // Keep email after failed attempt
                return View();
            }

            if (user.Email != email)
            {
                ModelState.AddModelError("", "Email is not Registered");
                return View();
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Index", "Product");
        }


        //Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

