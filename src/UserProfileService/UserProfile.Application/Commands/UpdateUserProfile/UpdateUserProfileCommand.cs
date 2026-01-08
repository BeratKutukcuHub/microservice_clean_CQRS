using MediatR;
using UserProfileService.Application.DTOs;
using UserProfileService.Domain.ValueObjects;

namespace UserProfileService.Application.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    Address? Address) : IRequest<UserProfileDto>;
