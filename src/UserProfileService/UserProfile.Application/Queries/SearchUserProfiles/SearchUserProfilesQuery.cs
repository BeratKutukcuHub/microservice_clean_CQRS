using MediatR;
using UserProfileService.Application.DTOs;

namespace UserProfileService.Application.Queries.SearchUserProfiles;

public record SearchUserProfilesQuery(string SearchTerm) : IRequest<List<UserProfileDto>>;
