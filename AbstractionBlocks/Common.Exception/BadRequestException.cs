namespace AbstractionBlocks.Common.Exception
{
    public class BadRequestException : BaseDomainException
    {
    public BadRequestException(string message = "Bad request") 
        : base(message, 400, "BAD_REQUEST") { }
    }
}
