using UserProfileService.Domain.ValueObjects;
namespace UserProfileService.Application.DTOs
{
    public record UserProfileDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        Address? Address);
}
