using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Helper;
using IdentityService.Application.Provider;
using IdentityService.Application.UOW;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using IdentityService.Identity.Domain;
using MediatR;
namespace IdentityService.Application.Auth.Identity.Commands
{
    public record AssignRoleCommand(Guid UserId, Guid RoleId, Guid TargetRoleId) : IRequest<TokenResponse>;
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, TokenResponse>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly IApplicationDispatcher _dispatcher;
        private readonly ILoggerService<AssignRoleCommandHandler> _logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AssignRoleCommandHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<AssignRoleCommandHandler> logger,
        IApplicationDispatcher dispatcher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _uow = uow;
            _logger = logger;
            _dispatcher = dispatcher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<TokenResponse> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _uow.IdentityUserAssingRoleAsync(request.UserId, request.RoleId, request.TargetRoleId);
            _logger.Information(
                "IdentityUser.AssignedRole",
                new
                {
                    ActorId = _uow.CurrentUser.UserId,
                    TargetId = request.UserId,
                    RoleId = request.RoleId,
                    TargetRoleId = request.TargetRoleId,
                }
            );
            var audit = AuditLog.Create(
                "IdentityUser",
                request.UserId,
                "AssignedRole",
                _uow.CurrentUser.UserId,
                _uow.CurrentUser.CorrelationId,
                "AssignRoleCommandHandler",
                new List<ChangeDetail>
                {
                    new ChangeDetail
                    {
                        Field = "Role",
                        NewValue = request.RoleId.ToString(),
                        OldValue = request.TargetRoleId.ToString()
                    }
                }
            );
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            var token = _jwtTokenGenerator.GenerateToken(result.User, result.Permissions);
            return new TokenResponse(token, result.User.LastRefreshToken(), DateTime.UtcNow);
        }
    }
}
