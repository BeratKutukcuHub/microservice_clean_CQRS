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
            _tokenService.CreateTokenAsync
            
        }
    }
}
