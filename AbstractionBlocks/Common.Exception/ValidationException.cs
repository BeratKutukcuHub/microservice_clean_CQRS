namespace AbstractionBlocks.Common.Exception
{
    public abstract class BaseDomainException : System.Exception
    {
        public int StatusCode { get; protected set; }
        public string ErrorCode { get; protected set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; protected set; }
        public string? CorrelationId { get; set; }

        protected BaseDomainException(string message, int statusCode = 500,
        string errorCode = "DOMAIN_ERROR",
        Dictionary<string, string[]>? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
        }
    }
    public class ValidationException : BaseDomainException
    {
        public ValidationException(string message, Dictionary<string, string[]> errors) : 
        base(message, 400, "VALIDATION_ERROR",
        errors){}
    }
}
