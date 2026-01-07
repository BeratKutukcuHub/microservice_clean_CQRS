using AbstractionBlocks.Common.Exception;
namespace IdentityService.Application.Exceptions
{
    public class NotFoundExceptionApp : NotFoundException
    {
        public NotFoundExceptionApp(string Id) : base($"The {Id} was not found.")
        {
        }
    }
}
