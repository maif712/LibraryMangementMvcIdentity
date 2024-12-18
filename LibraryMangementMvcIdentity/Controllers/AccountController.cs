using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMangementMvcIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _authService;

        public AccountController(IAccountService authService)
        {
            _authService = authService;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var result = await _authService.RegisterAsync(registerDto);
            if (result.IsSuccess)
            {
                return RedirectToAction("Login");
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(registerDto);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return View(loginDto);
            var result = await _authService.LoginAsync(loginDto);
            if (result.IsSuccess) return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(loginDto);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
