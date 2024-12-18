using System.ComponentModel.DataAnnotations;

namespace LibraryMangementMvcIdentity.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required,DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirme password")]
        public string ConfirmePassword { get; set; }
    }
}
