using ChickenWeb.Domain.Interfaces.IAccount;
using ChickenWeb.Models;
using ChickenWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ChickenWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _accountService.LoginAsync(model.Email, model.Password, model.Rememberme);
                    if (success)
                        return RedirectToAction("Index", "Home");

                    ModelState.AddModelError("", "Email or password is incorrect.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _accountService.RegisterAsync(model.FullName, model.Email, model.Password);
                    if (success)
                        return RedirectToAction("Index", "Home");

                    ModelState.AddModelError("", "Registration failed.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _accountService.GetUserByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email not found.");
                        return View(model);
                    }

                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("VerifyEmail", "Account");

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _accountService.ChangePasswordAsync(model.Email, model.NewPassword);
                    if (success)
                        return RedirectToAction("Login", "Account");

                    ModelState.AddModelError("", "Unable to change password.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Logout failed: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
