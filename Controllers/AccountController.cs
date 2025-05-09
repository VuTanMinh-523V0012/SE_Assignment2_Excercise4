using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Models;
using OrderManagement.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrderManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly OrderManagementContext _context;

        public AccountController(OrderManagementContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Try to find the user by email and password
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    if (user.IsLocked)
                    {
                        ModelState.AddModelError("", "Your account is locked. Please contact administrator.");
                        return View(model);
                    }

                    // Update last login time
                    user.LastLogin = DateTime.Now;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Store user information in the session
                    HttpContext.Session.SetInt32("UserID", user.UserID);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}