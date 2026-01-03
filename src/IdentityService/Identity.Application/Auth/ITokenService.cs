using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Application.Auth
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(string subject, IEnumerable<KeyValuePair<string, string>> claims);
    }
}