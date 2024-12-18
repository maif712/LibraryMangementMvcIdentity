namespace LibraryMangementMvcIdentity.Models.Entities
{
    public class Request
    {
        public string Id { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
        public string BookId { get; set; }
        public virtual Book Book { get; set; }
        public string RequestStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
