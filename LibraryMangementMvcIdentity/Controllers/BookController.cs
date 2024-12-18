using Azure;
using LibraryMangementMvcIdentity.Core.Interfaces;
using LibraryMangementMvcIdentity.Models.Dtos.Book;
using LibraryMangementMvcIdentity.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMangementMvcIdentity.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BaseBookDto baseBookDto)
        {
            if(!ModelState.IsValid) return View(baseBookDto);
            var result = await _bookService.CreateBookAsync(baseBookDto);
            if(result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Index");
            }

            TempData[StaticAlertMessage.ERROR] = result.Message;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(baseBookDto);

        }

        public async Task<IActionResult> Edit(string id)
        {
            var response = await _bookService.GetBookByIdAsync(id);
            if (response.IsSuccess) return View(response.Data);
            TempData[StaticAlertMessage.ERROR] = response.ErrorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, BaseBookDto baseBookDto)
        {
            var result = await _bookService.UpdateBookAsync(id, baseBookDto);
            if (result.IsSuccess)
            {
                TempData[StaticAlertMessage.SUCCESS] = result.Message;
                return RedirectToAction("Index");
            }
            TempData[StaticAlertMessage.ERROR] = result.Message;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(baseBookDto);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _bookService.GetBookByIdAsync(id);
            if(response.IsSuccess) return View(response.Data);

            TempData[StaticAlertMessage.ERROR] = response.ErrorMessage;
            return RedirectToAction("Index");

        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if( result.IsSuccess )
            {
                TempData[StaticAlertMessage.SUCCESS ] = result.Message;
                return RedirectToAction("Index");
            }

            TempData[StaticAlertMessage.ERROR] = result.Errors;
            return RedirectToAction("Index");
        }
    }
}
