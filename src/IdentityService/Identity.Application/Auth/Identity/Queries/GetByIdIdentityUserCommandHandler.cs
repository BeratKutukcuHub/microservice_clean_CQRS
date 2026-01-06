using AbstractionBlocks.Common.Exception.Logger;
using AutoMapper;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Queries
{
    public record GetByIdIdentityCommand(Guid Id) : IRequest<IdentityUserDto>;
    public class GetByIdIdentityUserCommandHandler : IRequestHandler<GetByIdIdentityCommand, IdentityUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerService<GetByIdIdentityUserCommandHandler> _logger;
        private readonly IApplicationDispatcher _dispatcher;
        public GetByIdIdentityUserCommandHandler(IUnitOfWork unitOfWork, ILoggerService<GetByIdIdentityUserCommandHandler> logger, IApplicationDispatcher dispatcher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task<IdentityUserDto> Handle(GetByIdIdentityCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null)
            {
                _logger.Warning(new NotFoundExceptionApp(request.Id.ToString()),
                "IdentityUser.GetById Id not found",
                new
                {
                    Action = "GetById",
                    ActorId = _unitOfWork.CurrentUser.UserId,
                    TargetId = request.Id
                });
            }
            var audit = AuditLog.Create("IdentityUser",
            request.Id, "GetById",
            _unitOfWork.CurrentUser.UserId,
            _unitOfWork.CurrentUser.CorrelationId,
            "GetByIdIdentityUserCommandHandler",
            null);
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            return new IdentityUserDto(result.Id, result.Name, result.Email);
        }
    }
}
