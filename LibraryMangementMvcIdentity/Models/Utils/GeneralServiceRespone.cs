namespace LibraryMangementMvcIdentity.Models.Utils
{
    public class GeneralServiceRespone
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
