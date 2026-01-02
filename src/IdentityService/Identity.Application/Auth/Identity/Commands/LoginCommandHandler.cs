using System.Text;
using AbstractBlocks.CommonDomain.Logger;
using AbstractionBlocks.CommonExceptionBase;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Helper;
using IdentityService.Application.Provider;
using IdentityService.Identity.Domain.Exceptions;
using IdentityService.Identity.Domain.Helper;
using IdentityService.Identity.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record LoginCommand(string email, string password) : IRequest<LoginResponse>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<LoginCommandHandler> _logger;
        private readonly IConfiguration _config;
        public LoginCommandHandler(IIdentityRepository identityRepository, IRoleRepository roleRepository, IConfiguration config,
        ILoggerService<LoginCommandHandler> logger)
        {
            _identityRepository = identityRepository;
            _roleRepository = roleRepository;
            _config = config;
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

            var tokenGenerator = new JwtTokenGenerator(_config);
            return new LoginResponse(tokenGenerator.GenerateToken(user, permissions.Distinct()), refreshToken);
        }
    }
}
