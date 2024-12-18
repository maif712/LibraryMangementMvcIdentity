using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMangementMvcIdentity.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public UserController(IUserService userService, UserManager<AppUser> userManager, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _userService.GetUserRequestsAsync(User);
            var response = await _accountService.GetUserInfoAsync(User);
            ViewBag.UserInfo = response.Data ?? new AppUser();
            return View(requests);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRequest(string id)
        {
            var result = await _userService.AddRequestAsync(User, id);
            if (result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = "Request was send successfully.";
                return RedirectToAction("Index", "Home");
            }

            TempData[StaticAlertMessage.ERROR] = HelperFunctions.ShowErrorMessage(result.Errors);
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Requests()
        {
            var requests = await _userService.GetAllRquestsAsync();
            return View(requests);
        }

        [HttpPost, ActionName("Confirm")]
        // Protect your form against Cross-Site Request Forgery (CSRF) attacks by using ASP.NET's built-in Anti-Forgery Token.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRequest(string rId, string bId)
        {
            var result = await _userService.ConfirmReqeustAsync(rId, bId);
            if (result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Requests");
            }
            TempData[StaticAlertMessage.ERROR] = HelperFunctions.ShowErrorMessage(result.Errors);
            return RedirectToAction("Requests");
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(string rId, string bId)
        {
            var result = await _userService.RejectRequestAsync(rId, bId);
            if(result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Requests");
            }
            TempData[StaticAlertMessage.ERROR] = HelperFunctions.ShowErrorMessage(result.Errors);
            return RedirectToAction("Requests");
        }

        // Admin can delete all the requests
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRequest(string id, string bId)
        {
            var result = await _userService.DeleteRequestAsync(id, bId);
            if (result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Requests");
            }
            TempData[StaticAlertMessage.ERROR] = HelperFunctions.ShowErrorMessage(result.Errors);
            return RedirectToAction("Requests");
        }

        // User can delete its own request
        [HttpPost, ActionName("UserDelete")]
        public async Task<IActionResult> DeleteRequestUser(string id, string bId)
        {
            var result = await _userService.DeleteRequestUserAsync(id, bId);
            if (result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            TempData[StaticAlertMessage.ERROR] = HelperFunctions.ShowErrorMessage(result.Errors);
            return RedirectToAction("Index");
        }
    }
}
