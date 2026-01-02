using AbstractBlocks.CommonDomain.Logger;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain.Repository;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Queries
{
    public record GetAllIdentityUsersCommand : IRequest<IEnumerable<IdentityUserDto>>;
    public class GetAllIdentityUsersCommandHandler : IRequestHandler<GetAllIdentityUsersCommand, IEnumerable<IdentityUserDto>>
    {
        private readonly ILoggerService<GetAllIdentityUsersCommandHandler> _logger;
        private readonly IIdentityRepository _identityRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GetAllIdentityUsersCommandHandler(IIdentityRepository identityRepository, ILoggerService<GetAllIdentityUsersCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _identityRepository = identityRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<IdentityUserDto>> Handle(GetAllIdentityUsersCommand request, CancellationToken cancellationToken)
        {
            var results = await _identityRepository.GetAllAsync();
            _logger.Information("IdentityUsers found. Count: {Count}", _unitOfWork.CurrentUser.UserId, Guid.NewGuid());
            return results.Select(x => new IdentityUserDto(x.Id, x.Name, x.Email, x.RoleIds.Select(x => x.ToString()), x.CreatedAt));
        }
    }
}
