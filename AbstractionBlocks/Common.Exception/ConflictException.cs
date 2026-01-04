namespace AbstractionBlocks.Common.Exception
{
    public class ConflictException : BaseDomainException
    {
        public ConflictException(string message = "Resource conflict") 
            : base(message, 409, "CONFLICT") { }
    }
}
