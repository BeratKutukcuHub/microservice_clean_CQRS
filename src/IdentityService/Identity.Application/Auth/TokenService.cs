using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application.Auth
{
    public class TokenService : ITokenService
    {
        private readonly ISecretProvider<JwtOptions> _secretProvider;
        public TokenService(ISecretProvider<JwtOptions> secretProvider)
        {
            _secretProvider = secretProvider;
        }

        public Task<string> CreateTokenAsync(string subject, IEnumerable<KeyValuePair<string, string>> claims)
        {
            var jwtSettings = _secretProvider.GetSection();
            var secret = jwtSettings?.SecretKey ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, subject)
            };
            foreach (var kv in claims)
            {
                claimList.Add(new Claim(kv.Key, kv.Value));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings?.Issuer,
                audience: jwtSettings?.Audience,
                claims: claimList,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}