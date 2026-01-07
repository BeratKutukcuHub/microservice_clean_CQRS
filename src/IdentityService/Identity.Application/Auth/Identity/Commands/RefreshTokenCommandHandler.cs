using AbstractionBlocks.Common.Exception;
using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Provider;
using IdentityService.Application.UOW;
using MediatR;
namespace IdentityService.Application.Auth.Identity.Commands
{
    public record RefreshTokenCommand(Guid refreshToken) : IRequest<TokenResponse>;
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<RefreshTokenCommandHandler> _logger;
        public RefreshTokenCommandHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<RefreshTokenCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = (await _uow.IdentityRepository.
            FindAsync(x => x.RefreshTokens.Any(x => x.Token == request.refreshToken))).FirstOrDefault();
            if (result is null) throw new NotFoundExceptionApp("Invalid User");
            var refresh = result.AddRefreshToken();
            await _uow.IdentityRepository.UpdateAsync(result);
            _logger.Information("IdentityUser.RefreshedToken", new { ActorId = result.Id });
            var token = _uow.JwtTokenGenerator.GenerateToken(result, await _uow.RoleRepository.GetAllPermissionsAsync(result.RoleIds));
            return new TokenResponse(token, refresh, DateTime.UtcNow);
        }
    }
}