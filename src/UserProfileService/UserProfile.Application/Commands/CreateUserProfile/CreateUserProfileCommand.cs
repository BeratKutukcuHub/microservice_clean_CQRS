using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Domain.ValueObjects;
namespace UserProfileService.Application.Commands.CreateUserProfile
{
    public record CreateUserProfileCommand(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        Address? Address) : IRequest<UserProfileDto>;
}
