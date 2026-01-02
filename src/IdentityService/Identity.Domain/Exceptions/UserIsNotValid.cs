using AbstractionBlocks.CommonExceptionBase;

namespace IdentityService.Identity.Domain.Exceptions
{
    public class UserIsNotValid : ValidationException
    {
        public UserIsNotValid(params string[] paramsErrorrs) : base(
            "The entered user information is incorrect.",
            new Dictionary<string, string[]> { { "errors", paramsErrorrs } }
        ) { }

    }
}
