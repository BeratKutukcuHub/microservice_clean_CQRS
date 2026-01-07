using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
namespace IdentityService.Application.Auth.Identity.Commands
{
    public record DeleteIdentityUserCommand(Guid Id, bool IsSoftDelete = false) : IRequest<bool>;
    public class DeleteIdentityUserCommandHandler : IRequestHandler<DeleteIdentityUserCommand, bool>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<DeleteIdentityUserCommandHandler> _logger;
        private readonly IApplicationDispatcher _dispatcher;
        public DeleteIdentityUserCommandHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<DeleteIdentityUserCommandHandler> logger,
        IApplicationDispatcher dispatcher)
        {
            _uow = uow;
            _logger = logger;
            _dispatcher = dispatcher;
        }
        public async Task<bool> Handle(DeleteIdentityUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _uow.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null) throw new NotFoundExceptionApp(request.Id.ToString());
            AuditLog auditLog;
            if (request.IsSoftDelete)
            {
                result.SoftDelete();
                await _uow.IdentityRepository.UpdateAsync(result);
                _logger.Information("IdentityUser.SoftDelete", new { ActorId = _uow.CurrentUser.UserId, TargetId = request.Id });
                auditLog = AuditLog.Create("IdentityUser",
                request.Id,
                "SoftDelete",
                _uow.CurrentUser.UserId,
                _uow.CurrentUser.CorrelationId,
                "DeleteIdentityUserCommandHandler",
                new List<ChangeDetail>
                {
                    new ChangeDetail
                    {
                        Field = "IsDeleted",
                        NewValue = "true",
                        OldValue = "false"
                    }
                });
            }
            else
            {
                await _uow.IdentityRepository.DeleteAsync(request.Id);
                _logger.Information("IdentityUser.HardDelete", new { ActorId = _uow.CurrentUser.UserId, TargetId = request.Id });
                auditLog = AuditLog.Create("IdentityUser",
                request.Id,
                "Delete",
                _uow.CurrentUser.UserId,
                _uow.CurrentUser.CorrelationId,
                "DeleteIdentityUserCommandHandler",
                new List<ChangeDetail>
                {
                    new ChangeDetail
                    {
                        Field = "IsDeleted",
                        NewValue = "true",
                        OldValue = "false"
                    }
                });
            }
            auditLog.AddAuditEvent();
            await _dispatcher.Dispatch(auditLog.Events);
            return true;
        }
    }
}
