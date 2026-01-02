using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Identity.Domain.Repository;
using MediatR;
using AbstractionBlocks.CommonExceptionBase;

namespace IdentityService.Application.Auth.Role.Queries
{
    public record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto>;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.Id);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.Id} not found.");

            return new RoleDto(role.Id, role.Name, role.Permissions);
        }
    }
}

