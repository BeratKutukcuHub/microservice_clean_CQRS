using AbstractBlocks.CommonDomain.Logger;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record CreateIdentityCommand(string? name, string email, string password) : IRequest<Guid>;
    public class CreateIdentityCommandHandler : IRequestHandler<CreateIdentityCommand, Guid>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly ILoggerService<CreateIdentityCommandHandler> _logService;
        public CreateIdentityCommandHandler(ILoggerService<CreateIdentityCommandHandler> logService,
        IIdentityRepository identityRepository)
        {
            _logService = logService;
            _identityRepository = identityRepository;
        }

        public async Task<Guid> Handle(CreateIdentityCommand request, CancellationToken cancellationToken)
        {
            var newlyUser = IdentityUser.Create(request.name ?? string.Empty, request.email, request.password);
            await _identityRepository.AddAsync(
                newlyUser);
            _logService.Information("IdentityUser created. Id: {IdentityId}", newlyUser.Id, null);
            return newlyUser.Id;
        }
    }
}
