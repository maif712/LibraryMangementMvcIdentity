namespace LibraryMangementMvcIdentity.Models.Dtos.Request
{
    public class BookRequestDto
    {
        public string AppUserId { get; set; }
        public string BookId { get; set; }
        public string RequestStatus { get; set; }
    }
}
