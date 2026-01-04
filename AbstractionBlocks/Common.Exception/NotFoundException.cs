namespace AbstractionBlocks.Common.Exception
{
    public class NotFoundException : BaseDomainException
    {
        public NotFoundException(string message = "Entity not found") : base(message, 404, "NOT_FOUND"){}
    }
}
