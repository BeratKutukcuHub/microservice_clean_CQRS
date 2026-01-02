namespace AbstractionBlocks.CommonExceptionBase
{
    public abstract class BaseDomainException : Exception
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
    public class BadRequestException : BaseDomainException
    {
    public BadRequestException(string message = "Bad request") 
        : base(message, 400, "BAD_REQUEST") { }
    }
    public class ForbiddenException : BaseDomainException
    {
    public ForbiddenException(string message = "Forbidden") 
        : base(message, 403, "FORBIDDEN") { }
    }
    public class ConflictException : BaseDomainException
    {
        public ConflictException(string message = "Resource conflict") 
            : base(message, 409, "CONFLICT") { }
    }
    public class NotFoundException : BaseDomainException
    {
        public NotFoundException(string message = "Entity not found") : base(message, 404, "NOT_FOUND"){}
    }
}
