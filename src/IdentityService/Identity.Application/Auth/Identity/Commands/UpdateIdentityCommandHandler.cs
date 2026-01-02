using System.Text.Json;
using AbstractBlocks.CommonDomain.Logger;
using AutoMapper;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public class UpdateIdentityCommand : IRequest<UpdateIdentityResponse> 
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    } 
    
    public class UpdateIdentityCommandHandler : IRequestHandler<UpdateIdentityCommand, UpdateIdentityResponse>
    {
        private ILoggerService<UpdateIdentityCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        public UpdateIdentityCommandHandler(
        ILoggerService<UpdateIdentityCommandHandler> logger,
        IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public async Task<UpdateIdentityResponse> Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"{request.Id} {request.Name} {request.Email} {request.Password}", request.Id, default);
            var result = await _uow.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null)
            {
                _logger.Warning("IdentityUser not found. Id: {IdentityId}", _uow.CurrentUser.UserId, "NotFound");
                throw new NotFoundExceptionApp(request.Id.ToString());
            }
            result.UpdateIdentity(request.Name, request.Email, request.Password);
            _logger.Information("IdentityUser updated. Id: {IdentityId}", request.Id, default);
            var response = await _uow.IdentityRepository.UpdateAsync(result);

            return new UpdateIdentityResponse(response.Id, response.Name, response.Email);
        }
    }
}
