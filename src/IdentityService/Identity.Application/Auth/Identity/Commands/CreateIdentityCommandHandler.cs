
using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record CreateIdentityCommand(string? name, string email, string password) : IRequest<Guid>;
    public class CreateIdentityCommandHandler : IRequestHandler<CreateIdentityCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<CreateIdentityCommandHandler> _logger;

        public CreateIdentityCommandHandler(IUnitOfWork uow, ILoggerService<CreateIdentityCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateIdentityCommand request, CancellationToken cancellationToken)
        {
            var id = await _uow.IdentityRepository.AddAsync(
                IdentityUser.Create(request.name ?? string.Empty, request.email, request.password));
            _logger.Information("IdentityUser.Created", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = id
            });
            return id;
        }
    }
}
