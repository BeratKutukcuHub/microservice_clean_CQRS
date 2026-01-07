using IdentityService.Identity.Domain;
namespace IdentityService.Application.Helper
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IdentityUser user, IEnumerable<string> permissions);
    }
}