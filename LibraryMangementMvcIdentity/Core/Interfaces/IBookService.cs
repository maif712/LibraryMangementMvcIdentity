using LibraryMangementMvcIdentity.Models.Dtos.Book;
using LibraryMangementMvcIdentity.Models.Entities;
using LibraryMangementMvcIdentity.Models.Utils;

namespace LibraryMangementMvcIdentity.Core.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<List<Book>> GetAllValidBooksAsync();
        Task<ServiceResponse<Book>> GetBookByIdAsync(string id);
        Task<GeneralServiceRespone> CreateBookAsync(BaseBookDto baseBookDto);
        Task<GeneralServiceRespone> UpdateBookAsync(string id, BaseBookDto baseBookDto);
        Task<GeneralServiceRespone> DeleteBookAsync(string id);

    }
}
