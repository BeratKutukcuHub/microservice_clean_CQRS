using AbstractionBlocks.Common.Exception;
using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Provider;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain.Helper;
using MediatR;


namespace IdentityService.Application.Auth.Identity.Commands
{
    public record LoginCommand(string email, string password) : IRequest<LoginResponse>;
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<LoginCommandHandler> _logger;
        public LoginCommandHandler(IUnitOfWork uow, ILoggerService<LoginCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = (await _uow.IdentityRepository.FindAsync(u => u.Email == request.email)).FirstOrDefault();
            if (result is null) throw new NotFoundExceptionApp(request.email);
            var isValid = PasswordHasher.VerifyPassword(request.password,
            Convert.FromBase64String(result.PasswordSalt),
            Convert.FromBase64String(result.PasswordHash));
            if (!isValid) throw new BadRequestException("Invalid Password");
            _logger.Information("IdentityUser.Login", new { ActorId = result.Id });
            var token = _uow.JwtTokenGenerator.GenerateToken(result, await _uow.RoleRepository.GetAllPermissionsAsync(result.RoleIds));
            var resultToken = result.LastRefreshToken();
            if (Guid.Empty == result.LastRefreshToken())
            {
                resultToken = result.AddRefreshToken();
                await _uow.IdentityRepository.UpdateAsync(result);
            }
            return new LoginResponse(token, resultToken);
        }
    }
}
