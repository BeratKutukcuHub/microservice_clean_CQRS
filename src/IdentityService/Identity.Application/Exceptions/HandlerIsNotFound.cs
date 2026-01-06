namespace IdentityService.Application.Exceptions
{
    public class HandlerIsNotFound : Exception
    {
        public HandlerIsNotFound(string handlerType) : base($"Handler {handlerType} is not found.")
        {
        }
    }
}