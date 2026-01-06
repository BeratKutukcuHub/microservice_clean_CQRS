namespace AbstractionBlocks.Common.Exception
{
    public class ConflictException : BaseDomainException
    {
        public ConflictException(string message = "Resource conflict", Guid? entityId = null, Guid? targetId = null) 
            : base(message, 409, "CONFLICT", entityId, targetId) { }
    }
}
