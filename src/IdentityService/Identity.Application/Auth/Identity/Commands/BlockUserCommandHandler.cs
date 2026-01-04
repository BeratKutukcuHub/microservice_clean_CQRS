using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.Helper;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Identity.Application.Auth.Identity.Commands
{
    public record BlockUserCommand(Guid Id, Guid correlationId) : IRequest<TokenResponse>;
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, TokenResponse>
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILoggerService<BlockUserCommandHandler> _logger;

        public BlockUserCommandHandler(IAuditRepository auditRepository,
        ICurrentUser currentUser,
        IIdentityRepository identityRepository,
        ILoggerService<BlockUserCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IRoleRepository roleRepository)
        {
            _auditRepository = auditRepository;
            _currentUser = currentUser;
            _identityRepository = identityRepository;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleRepository = roleRepository;
        }

        public async Task<TokenResponse> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
                var identity = await _identityRepository.GetByIdAsync(request.Id);
                if (identity == null) throw new NotFoundExceptionApp($"User with ID {request.Id} not found.");
                identity.BlockUser();
                var refresh = identity.AddRefreshToken();
                var resultIdentity = await _identityRepository.UpdateAsync(identity);
                await _auditRepository.AddAuditLogAsync(AuditLog.Create(
                "IdentityUser",
                request.Id,
                "Blocked",
                _currentUser.UserId,
                request.correlationId,
                "IdentityService",
                new List<ChangeDetail>
                {
                    new ChangeDetail
                    {
                        Field = "IsBlocked",
                        OldValue = "false",
                        NewValue = "true"
                    }
                }
                ));
                var resultRole = await _roleRepository.GetByIdAsync(request.Id);
                _logger.Information("IdentityUser blocked. Id: {IdentityId}", request.Id, correlationId: request.correlationId);
                return new TokenResponse(_jwtTokenGenerator.GenerateToken(resultIdentity, resultRole.Permissions), refresh
                    , DateTime.UtcNow
                );
            }
        }
}