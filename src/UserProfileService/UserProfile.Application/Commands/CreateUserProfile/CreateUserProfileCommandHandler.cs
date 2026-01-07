using AbstractionBlocks.Common.Application.Interfaces;
using AutoMapper;
using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Interfaces;
using UserProfileService.Domain.Entities;
namespace UserProfileService.Application.Commands.CreateUserProfile
{
    public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileDto>
    {
        private readonly IUserProfileRepository _repository;
        private readonly IMapper _mapper;
        private readonly IApplicationDispatcher _dispatcher;
        public CreateUserProfileCommandHandler(IUserProfileRepository repository, IMapper mapper, IApplicationDispatcher dispatcher)
        {
            _repository = repository;
            _mapper = mapper;
            _dispatcher = dispatcher;
        }
        public async Task<UserProfileDto> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = UserProfileService.Domain.Entities.UserProfile.Create(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.Address);
            var existingProfile = await _repository.GetByUserIdAsync(request.UserId);
            bool result;
            if (existingProfile == null)
            {
                result = await _repository.AddAsync(userProfile);
            }
            else
            {
                throw new InvalidOperationException($"User profile with UserId {request.UserId} already exists.");
            }
            if (result)
            {
                await _dispatcher.Dispatch(userProfile.Events);
            }
            return _mapper.Map<UserProfileDto>(userProfile);
        }
    }
}
