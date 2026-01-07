using AbstractionBlocks.Common.Exception;
namespace IdentityService.Application.Exceptions
{
    public class EventDispatchException : NotFoundException
    {
        public EventDispatchException(string message) : base(message)
        {
        }
    }
}