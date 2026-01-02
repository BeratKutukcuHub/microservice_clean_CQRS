using AbstractBlocks.CommonDomain.Logger;
using IdentityService.Application.UOW;
using MediatR;

namespace IdentityService.Application.Auth.Role.Commands
{
    public record DeleteRoleCommand(Guid Id) : IRequest<bool>;

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(IUnitOfWork uow, ILoggerService<DeleteRoleCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            await _uow.RoleRepository.DeleteAsync(request.Id);
            _logger.Information("Role deleted. Id: {RoleId}, DeletedBy: {UserId}", request.Id, _uow.CurrentUser.UserId);
            return true;
        }
    }
}

