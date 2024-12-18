namespace LibraryMangementMvcIdentity.Models.Utils
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static ServiceResponse<T> Success(T data)
        {
            return new ServiceResponse<T> { IsSuccess = true, Data = data};
        }

        public static ServiceResponse<T> Failure(string errorMessage = "")
        {
            return new ServiceResponse<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
