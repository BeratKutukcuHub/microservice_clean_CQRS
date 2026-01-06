namespace AbstractionBlocks.Common.Exception
{
    public class BadRequestException : BaseDomainException
    {
    public BadRequestException(string message = "Bad request", Guid? entityId = null, Guid? targetId = null) 
        : base(message, 400, "BAD_REQUEST", entityId, targetId) { }
    }
}
