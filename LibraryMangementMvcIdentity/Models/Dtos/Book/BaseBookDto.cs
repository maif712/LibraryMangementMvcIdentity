using System.ComponentModel.DataAnnotations;

namespace LibraryMangementMvcIdentity.Models.Dtos.Book
{
    public class BaseBookDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Author name")]
        public string Author { get; set; }
    }
}
