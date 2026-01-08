using AutoMapper;
using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Interfaces;

namespace UserProfileService.Application.Queries.SearchUserProfiles;

public class SearchUserProfilesQueryHandler : IRequestHandler<SearchUserProfilesQuery, List<UserProfileDto>>
{
    private readonly IUserProfileRepository _repository;
    private readonly IMapper _mapper;

    public SearchUserProfilesQueryHandler(IUserProfileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<UserProfileDto>> Handle(SearchUserProfilesQuery request, CancellationToken cancellationToken)
    {
        var allProfiles = await _repository.GetAllAsync();
        
        var searchTerm = request.SearchTerm.ToLower();
        var filteredProfiles = allProfiles
            .Where(p => !p.IsDeleted &&
                       (p.FirstName.ToLower().Contains(searchTerm) ||
                        p.LastName.ToLower().Contains(searchTerm) ||
                        p.Email.ToLower().Contains(searchTerm) ||
                        (p.PhoneNumber != null && p.PhoneNumber.Contains(searchTerm))))
            .OrderBy(p => p.FirstName)
            .ToList();

        return _mapper.Map<List<UserProfileDto>>(filteredProfiles);
    }
}
