using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Helper;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Provider;
using IdentityService.Application.UOW;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record RefreshTokenCommand(Guid Id,Guid refreshToken) : IRequest<TokenResponse>;
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        ICurrentUser _currentUser;
        IConfiguration _config;
        IUnitOfWork _uow;
        IJwtTokenGenerator _jwtTokenGenerator;
        ILoggerService<RefreshTokenCommandHandler> _logger;
        public RefreshTokenCommandHandler(ICurrentUser currentUser, IUnitOfWork uow, IConfiguration config, ILoggerService<RefreshTokenCommandHandler> logger, IJwtTokenGenerator jwtTokenGenerator)
        {
            _currentUser = currentUser;
            _uow = uow;
            _config = config;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if(request.Id != _currentUser.UserId) throw new UnauthorizedAccessException();
            var roles = await _uow.RoleRepository.GetByIdAsync(_currentUser.UserId);
            var identity = await _uow.IdentityRepository.GetByIdAsync(_currentUser.UserId);
            var result = identity.RefreshTokens.LastOrDefault(x => x.Expiry > DateTime.UtcNow &&
            x.Token == request.refreshToken);

            if (roles == null || identity == null || result == null) throw new UnauthorizedAccessException();
            var refreshToken = identity.AddRefreshToken();
            await _uow.IdentityRepository.UpdateAsync(identity);
            _logger.Information("IdentityUser refreshed. UserId: {UserId}, PerformedBy: {PerformedBy}",
                identity.Id,
                _currentUser.UserId);
            var newlyToken = _jwtTokenGenerator.GenerateToken(identity, roles.Permissions);
            return new TokenResponse(newlyToken, refreshToken, DateTime.UtcNow);
        }
    }
}