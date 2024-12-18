using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Dtos;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryMangementMvcIdentity.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountService> _logger;

        public AccountService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<AccountService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<GeneralServiceRespone> RegisterAsync(RegisterDto registerDto)
        {
            var isUserExists = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isUserExists != null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Message = "Registration falied.",
                    Errors = new List<string>() { "Username already taken." }
                };
            }

            AppUser newUser = new AppUser()
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (result.Succeeded)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = true,
                    Message = "Registration successful."
                };
            }


            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return new GeneralServiceRespone()
            {
                IsSuccess = false,
                Message = "",
                Errors = errors
            };
        }

        public async Task<GeneralServiceRespone> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, true);
            if (result.Succeeded)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = true,
                    Message = "Login was successful."
                };
            }

            return new GeneralServiceRespone()
            {
                IsSuccess = false,
                Message = "Login failed.",
                Errors = new List<string>() { "Username or password is wrong, try again!" }
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ServiceResponse<AppUser>> GetUserInfoAsync(ClaimsPrincipal user)
        {
            var userInfo = await _userManager.GetUserAsync(user);
            if (userInfo == null)
            {
                return ServiceResponse<AppUser>.Failure("No user found.");
            }

            try
            {
                return ServiceResponse<AppUser>.Success(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user info.");
                return ServiceResponse<AppUser>.Failure("Database error");
                throw;
            }
        }
    }
}
