namespace AbstractionBlocks.Common.Exception
{
    public class ForbiddenException : BaseDomainException
    {
    public ForbiddenException(string message = "Forbidden") 
        : base(message, 403, "FORBIDDEN") { }
    }
}
