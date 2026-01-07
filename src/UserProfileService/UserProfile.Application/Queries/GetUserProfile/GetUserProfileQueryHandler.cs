using AutoMapper;
using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Interfaces;
namespace UserProfileService.Application.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IUserProfileRepository _repository;
        private readonly IMapper _mapper;
        public GetUserProfileQueryHandler(IUserProfileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _repository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<UserProfileDto>(profile);
        }
    }
}
