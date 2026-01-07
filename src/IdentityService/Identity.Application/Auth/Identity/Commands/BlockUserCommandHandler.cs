using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Helper;
using IdentityService.Application.Provider;
using IdentityService.Application.UOW;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
namespace IdentityService.Identity.Application.Auth.Identity.Commands
{
    public record BlockUserCommand(Guid Id) : IRequest<TokenResponse>;
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, TokenResponse>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<BlockUserCommandHandler> _logger;
        private readonly IApplicationDispatcher _dispatcher;
        public BlockUserCommandHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<BlockUserCommandHandler> logger, IApplicationDispatcher dispatcher)
        {
            _uow = uow;
            _logger = logger;
            _dispatcher = dispatcher;
        }
        public async Task<TokenResponse> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _uow.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null) throw new NotFoundExceptionApp(request.Id.ToString());
            result.BlockUser();
            var refresh = result.AddRefreshToken();
            await _uow.IdentityRepository.UpdateAsync(result);
            _logger.Information("IdentityUser.Blocked", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = request.Id
            });
            var audit = AuditLog.Create(
                "IdentityUser",
                request.Id,
                "Blocked",
                _uow.CurrentUser.UserId,
                _uow.CurrentUser.CorrelationId,
                "BlockUserCommandHandler",
                new List<ChangeDetail>
                {
                    new ChangeDetail
                    {
                        Field = "Blocked",
                        NewValue = "true",
                        OldValue = "false"
                    }
                }
            );
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            var token = _uow.JwtTokenGenerator.GenerateToken(result, await _uow.RoleRepository.GetAllPermissionsAsync(result.RoleIds));
            return new TokenResponse(token, refresh, DateTime.UtcNow);
        }
    }
}