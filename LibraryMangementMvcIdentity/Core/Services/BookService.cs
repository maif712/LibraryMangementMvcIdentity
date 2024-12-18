using LibraryMangementMvcIdentity.Core.Data;
using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Dtos.Book;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;
using Microsoft.EntityFrameworkCore;

namespace LibraryMangementMvcIdentity.Core.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookService> _logger;

        public BookService(ApplicationDbContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .OrderByDescending(book => book.CreatedAt)
                .ToListAsync();
            return books;
        }

        public async Task<List<Book>> GetAllValidBooksAsync()
        {
            var validBook = await _context.Books
                .Where(b => b.Status == StaticStatus.AVAILABLE)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return validBook;
        }

        public async Task<ServiceResponse<Book>> GetBookByIdAsync(string id)
        {
            if(string.IsNullOrEmpty(id))
                return ServiceResponse<Book>.Failure("The book ID cannot be null or empty.");

            var book = await _context.Books.FindAsync(id);
            if(book == null)
                return ServiceResponse<Book>.Failure("No book found");

            return ServiceResponse<Book>.Success(book);
        }
        public async Task<GeneralServiceRespone> CreateBookAsync(BaseBookDto baseBookDto)
        {
            Book newBook = new Book()
            {
                Id = Guid.NewGuid().ToString(),
                Title = baseBookDto.Title,
                Description = baseBookDto.Description,
                Author = baseBookDto.Author,
                Status = StaticStatus.AVAILABLE,
                CreatedAt = DateTime.Now
            };
            try
            {
                await _context.Books.AddAsync(newBook);
                await _context.SaveChangesAsync();
                return new GeneralServiceRespone()
                {
                    IsSuccess = true,
                    Message = "New book created successfully.",
                };
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while creating the book.");
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Message = "An error occurred while adding new books.",
                    Errors = new List<string>() { "Database failed." }
                };
            }
        }

        public async Task<GeneralServiceRespone> UpdateBookAsync(string id, BaseBookDto baseBookDto)
        {
            if(string.IsNullOrEmpty(id))
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "The book ID cannot be null or empty." }
                };

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if(book == null)
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "No book found" }
                };


            book.Title = baseBookDto.Title;
            book.Description = baseBookDto.Description;
            book.Author = baseBookDto.Author;

            try
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return new GeneralServiceRespone()
                {
                    IsSuccess = true,
                    Message = "New book updated successfully.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book.");
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred while updating the book. Please try again later.",
                    Errors = new List<string>() { "Database update failed." }
                };
            }

        }

        public async Task<GeneralServiceRespone> DeleteBookAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "The book ID cannot be null or empty." }
                };

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "No book found" }
                };

            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return new GeneralServiceRespone()
                {
                    IsSuccess = true,
                    Message = "book removed successfully.",
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the book.");
                return new GeneralServiceRespone()
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred while removing the book. Please try again later.",
                    Errors = new List<string>() { "Database failed." }
                };
            }
        }


        
    }
}
