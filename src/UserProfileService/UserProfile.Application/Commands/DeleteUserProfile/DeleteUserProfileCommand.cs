using MediatR;

namespace UserProfileService.Application.Commands.DeleteUserProfile;

public record DeleteUserProfileCommand(Guid UserId) : IRequest<bool>;
