namespace AbstractionBlocks.Common.Exception
{
    public class ValidationException : BaseDomainException
    {
        public ValidationException(string message, Dictionary<string, string[]> errors, Guid? entityId = null, Guid? targetId = null) : 
        base(message, 400, "VALIDATION_ERROR", entityId, targetId,
        errors){}
    }
}
