namespace SCED.API.Common
{
    public class ServiceResponse<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
        
        public ServiceResponse() { }
        
        public ServiceResponse(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ServiceResponse<T> CreateSuccess(T data, string message = "Success")
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResponse<T> CreateError(string message)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Data = null
            };
        }

        public static ServiceResponse<T> CreateError(string message, T data)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }
}