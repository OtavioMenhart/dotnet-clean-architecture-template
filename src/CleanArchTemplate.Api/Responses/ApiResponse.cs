namespace CleanArchTemplate.Api.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? Message { get; set; }

        public ApiResponse(T? data, string? message = null)
        {
            Data = data;
            Message = message;
        }
    }
}
