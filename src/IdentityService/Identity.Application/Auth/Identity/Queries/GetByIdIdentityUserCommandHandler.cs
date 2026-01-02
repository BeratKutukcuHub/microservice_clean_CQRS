using AbstractBlocks.CommonDomain.Logger;
using AutoMapper;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Queries
{
    public record GetByIdIdentityCommand(Guid Id) : IRequest<IdentityUserDto>;
    public class GetByIdIdentityUserCommandHandler : IRequestHandler<GetByIdIdentityCommand, IdentityUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerService<GetByIdIdentityUserCommandHandler> _logger;

        public GetByIdIdentityUserCommandHandler(IUnitOfWork unitOfWork, ILoggerService<GetByIdIdentityUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IdentityUserDto> Handle(GetByIdIdentityCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null)
            {
                _logger.Warning("IdentityUser not found. Id: {IdentityId}", request.Id, "NotFound");
                throw new NotFoundExceptionApp(request.Id.ToString());
            }
            var roleNames = await _unitOfWork.RoleRepository.RolesOfUserAsync(result.RoleIds);
            _logger.Information("IdentityUser found. Id: {IdentityId}", request.Id, Guid.NewGuid());
            return new IdentityUserDto(result.Id, result.Name, result.Email, roleNames ?? Enumerable.Empty<string>(), result.CreatedAt);
        }
    }
}
