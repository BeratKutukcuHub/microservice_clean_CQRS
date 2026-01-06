using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Pagination;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Queries
{
    public record GetAllIdentityUsersCommand(PaginationValue pag) : IRequest<PaginationResponse<IdentityUser>>;
    public class GetAllIdentityUsersCommandHandler : IRequestHandler<GetAllIdentityUsersCommand, PaginationResponse<IdentityUser>>
    {
        private readonly ILoggerService<GetAllIdentityUsersCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDispatcher _dispatcher;
        public GetAllIdentityUsersCommandHandler(ILoggerService<GetAllIdentityUsersCommandHandler> logger, IUnitOfWork unitOfWork, IApplicationDispatcher dispatcher)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dispatcher = dispatcher;
        }

        public async Task<PaginationResponse<IdentityUser>> Handle(GetAllIdentityUsersCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.IdentityRepository.GetAllPagination(request.pag);
            var audit = AuditLog.Create(
                "IdentityUser",
                null,
                "GetAll",
                _unitOfWork.CurrentUser.UserId,
                _unitOfWork.CurrentUser.CorrelationId,
                "GetAllIdentityUsersCommandHandler",
                null
            );
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            return result;
        }
    }
}
