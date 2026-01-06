namespace AbstractionBlocks.Common.Exception
{
    public class NotFoundException : BaseDomainException
    {
        public NotFoundException(string message = "Entity not found", Guid? entityId = null, Guid? targetId = null) 
        : base(message, 404, "NOT_FOUND", entityId, targetId){}
    }
}
