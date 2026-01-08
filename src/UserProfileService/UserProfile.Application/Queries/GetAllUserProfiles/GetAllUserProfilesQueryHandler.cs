using AbstractionBlocks.Common.Pagination;
using AutoMapper;
using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Interfaces;

namespace UserProfileService.Application.Queries.GetAllUserProfiles;

public class GetAllUserProfilesQueryHandler : IRequestHandler<GetAllUserProfilesQuery, PaginationResponse<UserProfileDto>>
{
    private readonly IUserProfileRepository _repository;
    private readonly IMapper _mapper;

    public GetAllUserProfilesQueryHandler(IUserProfileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<UserProfileDto>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
    {
        var allProfiles = await _repository.GetAllAsync();
        
        var activeProfiles = allProfiles
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        var totalCount = activeProfiles.Count;
        var pagedProfiles = activeProfiles
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var profileDtos = _mapper.Map<List<UserProfileDto>>(pagedProfiles);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return PaginationResponse<UserProfileDto>.Create(
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages,
            profileDtos
        );
    }
}
