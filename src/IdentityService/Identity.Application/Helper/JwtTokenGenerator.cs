using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using IdentityService.Identity.Domain;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application.Helper
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly ISecretProvider<JwtOptions> _config;

        public JwtTokenGenerator(ISecretProvider<JwtOptions> config)
        {
            _config = config;
        }

        private List<Claim> SetClaims(IdentityUser user, IEnumerable<string> permissions)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            foreach (var role in user.RoleIds)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            return claims;
        }

        private JwtSecurityToken CreateToken(string secretKey, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature)
            );
        }

        public string GenerateToken(IdentityUser user, IEnumerable<string> permissions)
        {
            return new JwtSecurityTokenHandler().WriteToken(
                CreateToken(_config.GetSection().SecretKey ?? string.Empty, SetClaims(user, permissions))
            );
        }
    }
}
