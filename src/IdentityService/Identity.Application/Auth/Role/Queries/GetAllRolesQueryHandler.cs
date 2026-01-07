using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using MediatR;
namespace IdentityService.Application.Auth.Role.Queries
{
    public record GetAllRolesQuery() : IRequest<IEnumerable<RoleDto>>;
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<GetAllRolesQueryHandler> _logger;
        public GetAllRolesQueryHandler(IdentityService.Application.UOW.IUnitOfWork uow, ILoggerService<GetAllRolesQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _uow.RoleRepository.GetAllAsync();
            _logger.Information("Roles.GetAll", new { ActorId = _uow.CurrentUser.UserId });
            return roles.Select(r => new RoleDto(r.Id, r.Name, r.Permissions));
        }
    }
}
