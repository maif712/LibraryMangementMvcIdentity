using LibraryMangementMvcIdentity.Models.Dtos;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using System.Security.Claims;

namespace LibraryMangementMvcIdentity.Core.Interfaces
{
    public interface IAccountService
    {
        Task<GeneralServiceRespone> RegisterAsync(RegisterDto registerDto);
        Task<GeneralServiceRespone> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<AppUser>> GetUserInfoAsync(ClaimsPrincipal user);
        Task LogoutAsync();
    }
}
