using AbstractBlocks.CommonDomain.Logger;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain.Repository;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record DeleteIdentityUserCommand(Guid Id, bool IsSoftDelete = false) : IRequest<bool>;
    public class DeleteIdentityUserCommandHandler : IRequestHandler<DeleteIdentityUserCommand, bool>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly ILoggerService<DeleteIdentityUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteIdentityUserCommandHandler(IIdentityRepository identityRepository, ILoggerService<DeleteIdentityUserCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _identityRepository = identityRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteIdentityUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityRepository.GetByIdAsync(request.Id);
            if (result is null) throw new NotFoundExceptionApp(request.Id.ToString());
            if (request.IsSoftDelete)
            {
                result.SoftDelete();
                await _identityRepository.UpdateAsync(result);
                _logger.Information("IdentityUser soft-deleted. TargetUserId: {TargetUserId}, PerformedBy: {PerformedBy}",
                request.Id,
                _unitOfWork.CurrentUser.UserId);
                return true;
            }
            else
            {
                await _identityRepository.DeleteAsync(request.Id);
                _logger.Information("IdentityUser deleted. TargetUserId: {TargetUserId}, PerformedBy: {PerformedBy}",
                request.Id,
                _unitOfWork.CurrentUser.UserId);
                return true;
            }
        }
    }
}
