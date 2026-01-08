using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Pagination;
using MediatR;
using UserProfileService.Application.DTOs;

namespace UserProfileService.Application.Queries.GetAllUserProfiles;

[Cache("all-userprofiles", 10)]
public record GetAllUserProfilesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PaginationResponse<UserProfileDto>>;
