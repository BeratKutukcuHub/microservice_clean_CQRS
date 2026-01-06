namespace AbstractionBlocks.Common.Exception
{
    public class ForbiddenException : BaseDomainException
    {
    public ForbiddenException(string message = "Forbidden", Guid? entityId = null, Guid? targetId = null) 
        : base(message, 403, "FORBIDDEN", entityId, targetId) { }
    }
}
