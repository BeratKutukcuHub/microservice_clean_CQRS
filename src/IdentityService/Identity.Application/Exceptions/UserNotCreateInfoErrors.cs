using AbstractionBlocks.Common.Exception;

namespace IdentityService.Application.Exceptions
{
    public class UserNotCreateInfoErrors : ValidationException
    {
        public UserNotCreateInfoErrors(
            string message,
            Dictionary<string, string[]> errors
        ) : base(message, errors)
        {

        }
    }
}
