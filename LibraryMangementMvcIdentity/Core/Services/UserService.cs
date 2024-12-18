using LibraryMangementMvcIdentity.Core.Data;
using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Dtos.Request;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LibraryMangementMvcIdentity.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<AppUser> userManager, ApplicationDbContext context, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<List<Request>> GetAllRquestsAsync()
        {
            var requests = await _context.Requests
                .Include(r => r.AppUser)
                .Include(r => r.Book)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
            return requests;
        }

        public async Task<List<Request>> GetUserRequestsAsync(ClaimsPrincipal user)
        {
            var uId = _userManager.GetUserId(user);
            if (uId == null) return null;


            var requests = await _context.Requests
                .Where(r => r.AppUserId == uId)
                .OrderByDescending(r => r.RequestDate)
                .Include(r => r.Book)
                .ToListAsync();
            return requests;
        }
        public async Task<GeneralServiceRespone> AddRequestAsync(ClaimsPrincipal user, string bookId)
        {
            var uId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(uId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "No user found." }
                };
            }

            if (string.IsNullOrEmpty(bookId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Book id can not be null or empty." }
                };
            }

            // Handling race condition for sending request.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var book = await _context.Books.FindAsync(bookId);
                    if (book == null)
                    {
                        return new GeneralServiceRespone()
                        {
                            IsSuccess = false,
                            Errors = new List<string>() { "No book found." }
                        };
                    }

                    if (book.Status != StaticStatus.AVAILABLE)
                    {
                        return new GeneralServiceRespone()
                        {
                            IsSuccess = false,
                            Errors = new List<string>() { "Book is already reserved or not available." }
                        };
                    }

                    book.Status = StaticStatus.PENDING;
                    Request newRequest = new Request()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AppUserId = uId,
                        BookId = bookId,
                        RequestStatus = StaticStatus.PENDING,
                        RequestDate = DateTime.Now,
                    };

                    await _context.Requests.AddAsync(newRequest);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new GeneralServiceRespone()
                    {
                        IsSuccess = true,
                        Message = "New reqeust added successfully."
                    };


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "An error occurred while sending request.");
                    return new GeneralServiceRespone()
                    {
                        IsSuccess = false,
                        Message = "An error occurred while sending request.",
                        Errors = new List<string>() { "Database failed." }
                    };
                }
            }
        }

        public async Task<GeneralServiceRespone> ConfirmReqeustAsync(string rId, string bId)
        {
            if (string.IsNullOrWhiteSpace(rId) || string.IsNullOrWhiteSpace(bId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Id can not  be null" }
                };
            }

            var book = await _context.Books.FindAsync(bId);
            if (book == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No book found"
                    }
                };
            }
            var request = await _context.Requests.FindAsync(rId);
            if (request == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No request found"
                    }
                };
            }

            request.RequestStatus = StaticStatus.CONFIRMED;
            book.Status = StaticStatus.BORROWED;
            await _context.SaveChangesAsync();
            return new GeneralServiceRespone()
            {
                IsSuccess = true,
                Message = "Book request confimred successfully."
            };
        }

        public async Task<GeneralServiceRespone> RejectRequestAsync(string rId, string bId)
        {
            if (string.IsNullOrWhiteSpace(rId) || string.IsNullOrWhiteSpace(bId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Id can not  be null" }
                };
            }

            var book = await _context.Books.FindAsync(bId);
            if (book == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No book found"
                    }
                };
            }
            var request = await _context.Requests.FindAsync(rId);
            if (request == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No request found"
                    }
                };
            }

            request.RequestStatus = StaticStatus.REJECTED;
            book.Status = StaticStatus.AVAILABLE;
            await _context.SaveChangesAsync();
            return new GeneralServiceRespone()
            {
                IsSuccess = true,
                Message = "Book request confimred successfully."
            };
        }

        // For admin only
        public async Task<GeneralServiceRespone> DeleteRequestAsync(string requestId, string bId)
        {
            if (string.IsNullOrEmpty(requestId) || string.IsNullOrWhiteSpace(bId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Id can not be null." }
                };
            }

            var request = await _context.Requests.FindAsync(requestId);
            if (request == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "No request found." }
                };
            }

            var book = await _context.Books.FindAsync(bId);
            if (book == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No book found"
                    }
                };
            }

            book.Status = StaticStatus.AVAILABLE;
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return new GeneralServiceRespone()
            {
                IsSuccess = true,
                Message = "Reqeust removed successfully."
            };
        }


        // For user, Only requests that its status is pending.
        public async Task<GeneralServiceRespone> DeleteRequestUserAsync(string requestId, string bId)
        {
            if (string.IsNullOrEmpty(requestId) || string.IsNullOrWhiteSpace(bId))
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Id can not be null." }
                };
            }

            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == StaticStatus.PENDING);
            if (request == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "No request found." }
                };
            }

            var book = await _context.Books.FindAsync(bId);
            if (book == null)
            {
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>()
                    {
                        "No book found"
                    }
                };
            }

            book.Status = StaticStatus.AVAILABLE;
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return new GeneralServiceRespone()
            {
                IsSuccess = true,
                Message = "Reqeust removed successfully."
            };
        }
    }
}
