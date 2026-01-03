using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain.Helper;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record LoginCommand(string email, string password) : IRequest<LoginResponse>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<LoginCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(IIdentityRepository identityRepository, IRoleRepository roleRepository, ITokenService tokenService,
        ILoggerService<LoginCommandHandler> logger)
        {
            _identityRepository = identityRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = (await _identityRepository.FindAsync(u => u.Email == request.email)).FirstOrDefault();

            if (user == null)
            {
                _logger.Warning("Login failed. Email not found: {Email}", request.email, "AuthFail");
                throw new BadRequestException($"{request.email} email is not registered.");
            }

            var isPasswordValid = PasswordHasher.VerifyPassword(
                request.password,
                Convert.FromBase64String(user.PasswordSalt),
                Convert.FromBase64String(user.PasswordHash));

            if (!isPasswordValid)
            {
                _logger.Warning("Login failed. Invalid password for: {Email}", request.email, "AuthFail");
                throw new BadRequestException("Invalid credentials.");
            }

            _logger.Information("Login successful for: {Email}", user.Id);

            var permissions = new List<string>();
            foreach (var roleId in user.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null)
                {
                    permissions.AddRange(role.Permissions);
                }
            }

            var refreshToken = user.AddRefreshToken();
            await _identityRepository.UpdateAsync(user);

            var claims = new List<KeyValuePair<string,string>>
            {
                new KeyValuePair<string,string>(System.Security.Claims.ClaimTypes.Name, user.Name),
                new KeyValuePair<string,string>(System.Security.Claims.ClaimTypes.Email, user.Email),
                new KeyValuePair<string,string>(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach(var roleId in user.RoleIds)
            {
                claims.Add(new KeyValuePair<string,string>(System.Security.Claims.ClaimTypes.Role, roleId.ToString()));
            }
            foreach(var p in permissions.Distinct())
            {
                claims.Add(new KeyValuePair<string,string>("permission", p));
            }

            var token = await _tokenService.CreateTokenAsync(user.Id.ToString(), claims);
            return new LoginResponse(token, refreshToken);
        }
    }
}
