using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Pagination;
using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AutoMapper;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MediatR;
namespace IdentityService.Application.Auth.Identity.Queries
{
    [Cache("IdentityUsersList", 2)]
    public record GetAllIdentityUsersCommand(PaginationValue pag) : IRequest<PaginationResponse<GetIdentityUser>>;
    public class GetAllIdentityUsersCommandHandler : IRequestHandler<GetAllIdentityUsersCommand, PaginationResponse<GetIdentityUser>>
    {
        private readonly ILoggerService<GetAllIdentityUsersCommandHandler> _logger;
        private readonly IdentityService.Application.UOW.IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly IApplicationDispatcher _dispatcher;
        public GetAllIdentityUsersCommandHandler(ILoggerService<GetAllIdentityUsersCommandHandler> logger, IdentityService.Application.UOW.IUnitOfWork unitOfWork, IApplicationDispatcher dispatcher, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dispatcher = dispatcher;
            this.mapper = mapper;
        }
        public async Task<PaginationResponse<GetIdentityUser>> Handle(GetAllIdentityUsersCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.IdentityRepository.GetAllPagination(request.pag);
            if (result is null) throw new NotFoundExceptionApp("IdentityUser");
            var audit = AuditLog.Create(
                "IdentityUser",
                Guid.Empty,
                "GetAll",
                _unitOfWork.CurrentUser.UserId,
                _unitOfWork.CurrentUser.CorrelationId,
                "GetAllIdentityUsersCommandHandler",
                null
            );
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            return PaginationResponse<GetIdentityUser>.Create(
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages, mapper.Map<List<GetIdentityUser>>(result.Data));
        }
    }
}
