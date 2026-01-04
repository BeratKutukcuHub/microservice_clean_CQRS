using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Provider;
using IdentityService.Identity.Application.Repository;
using MediatR;
using IdentityService.Application.Helper;
using IdentityService.Identity.Domain;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record LoginCommand(string email, string password) : IRequest<LoginResponse>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<LoginCommandHandler> _logger;
        private readonly IJwtTokenGenerator _tokenService;
        private readonly IApplicationDispatcher _dispatcher;
        private readonly ICurrentUser _currentUser;
        public LoginCommandHandler(IIdentityRepository identityRepository, IRoleRepository roleRepository, IJwtTokenGenerator tokenService,
        ILoggerService<LoginCommandHandler> logger, IApplicationDispatcher dispatcher, ICurrentUser currentUser)
        {
            _identityRepository = identityRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _logger = logger;
            _dispatcher = dispatcher;
            _currentUser = currentUser;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = (await _identityRepository.FindAsync(u => u.Email == request.email)).FirstOrDefault();
            var roles = await _roleRepository.FindAsync(x => user.RoleIds.Contains(x.Id));
            var check = roles.Any(x => x.Name == "Admin");
            _logger.Information("Login successful for: {Email}", default, default);
            var token = _tokenService.GenerateToken(user, roles.SelectMany(x => x.Permissions));
            var result = user.RefreshTokens.LastOrDefault();
            bool isRefreshTokenExist = result is null;
            Guid refreshToken = isRefreshTokenExist ? user.AddRefreshToken() : result.Token;
            if(isRefreshTokenExist) await _identityRepository.UpdateAsync(user);
            return new LoginResponse(token , refreshToken);
        }
    }
}
