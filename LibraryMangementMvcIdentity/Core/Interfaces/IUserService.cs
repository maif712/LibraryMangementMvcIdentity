using LibraryMangementMvcIdentity.Models.Dtos.Request;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using System.Security.Claims;

namespace LibraryMangementMvcIdentity.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<Request>> GetAllRquestsAsync();
        Task<List<Request>> GetUserRequestsAsync(ClaimsPrincipal user);
        Task<GeneralServiceRespone> AddRequestAsync(ClaimsPrincipal user, string bookId);
        Task<GeneralServiceRespone> ConfirmReqeustAsync(string requestId, string bookId);
        Task<GeneralServiceRespone> RejectRequestAsync(string requestId, string bookId);
        Task<GeneralServiceRespone> DeleteRequestAsync(string requestId, string bookId);

        Task<GeneralServiceRespone> DeleteRequestUserAsync(string requestId, string bookId);
    }
}
