namespace AbstractionBlocks.Common.Exception
{
    public abstract class BaseDomainException : System.Exception
    {
        public Guid EntityId { get; protected set; }
        public Guid TargetId { get; protected set; }
        public int StatusCode { get; protected set; }
        public string ErrorCode { get; protected set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; protected set; }
        public string? CorrelationId { get; set; }
        protected BaseDomainException(string message, int statusCode = 500,
        string errorCode = "DOMAIN_ERROR", Guid? entityId = null, Guid? targetId = null,
        Dictionary<string, string[]>? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
            EntityId = entityId ?? Guid.Empty;
            TargetId = targetId ?? Guid.Empty;
        }
    }
}
