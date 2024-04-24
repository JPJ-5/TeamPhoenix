public class ApiResponse<T>
{
    public string Status { get; set; } = "Success";
    public string Message { get; set; } = "Operation completed successfully.";
    public T Data { get; set; }

    public ApiResponse(T data, string? message = null)
    {
        Data = data;
        if (message != null)
        {
            Message = message;
        }
    }
}