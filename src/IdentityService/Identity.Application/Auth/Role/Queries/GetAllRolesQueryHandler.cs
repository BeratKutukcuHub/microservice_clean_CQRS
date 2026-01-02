using AbstractBlocks.CommonDomain.Logger;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using MediatR;

namespace IdentityService.Application.Auth.Role.Queries
{
    public record GetAllRolesQuery() : IRequest<IEnumerable<RoleDto>>;

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<GetAllRolesQueryHandler> _logger;

        public GetAllRolesQueryHandler(IUnitOfWork uow, ILoggerService<GetAllRolesQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _uow.RoleRepository.GetAllAsync();
            _logger.Information("Roles retrieved. Count: {Count}, RequestedBy: {UserId}", _uow.CurrentUser.UserId, Guid.Empty);
            return roles.Select(r => new RoleDto(r.Id, r.Name, r.Permissions));
        }
    }
}

