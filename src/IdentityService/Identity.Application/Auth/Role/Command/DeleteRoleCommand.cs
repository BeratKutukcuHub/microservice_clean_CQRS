using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.UOW;
using MediatR;
namespace IdentityService.Application.Auth.Role.Commands
{
    public record DeleteRoleCommand(Guid Id) : IRequest<bool>;
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<DeleteRoleCommandHandler> _logger;
        public DeleteRoleCommandHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<DeleteRoleCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            await _uow.RoleRepository.DeleteAsync(request.Id);
            _logger.Information("Role.Deleted", new
            {
                Action = "Delete",
                ActorId = _uow.CurrentUser.UserId,
                TargetId = request.Id
            });
            return true;
        }
    }
}
