namespace api_template.Middleware.Wrappers
{
    [Serializable]
    public class ApiError
    {
        public string ExceptionMessage { get; set; }
        public string? Details { get; set; }

        public ApiError(string exceptionMessage, string? details = null)
        {
            ExceptionMessage = exceptionMessage;
            Details = details;
        }
    }
}
