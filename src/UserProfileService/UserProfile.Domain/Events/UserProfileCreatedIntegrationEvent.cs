using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace UserProfileService.Domain.Events;

public class UserProfileCreatedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid ProfileId { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }

    public UserProfileCreatedIntegrationEvent(
        Guid profileId,
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber) : base("UserProfileService", "v1")
    {
        ProfileId = profileId;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
