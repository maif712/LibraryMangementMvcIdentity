using Microsoft.AspNetCore.Identity;

namespace LibraryMangementMvcIdentity.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public virtual List<Request> Requests { get; set; }
    }
}
