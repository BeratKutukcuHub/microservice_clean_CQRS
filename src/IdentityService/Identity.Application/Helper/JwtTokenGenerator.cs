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
        private JwtSecurityToken CreateToken(List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _config.GetSection().Issuer,
                audience: _config.GetSection().Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(_config.GetSection().SecretKey)),
                SecurityAlgorithms.HmacSha256Signature)
            );
        }
        public string GenerateToken(IdentityUser user, IEnumerable<string> permissions)
        {
            return new JwtSecurityTokenHandler().WriteToken(
                CreateToken(SetClaims(user, permissions))
            );
        }
    }
}
