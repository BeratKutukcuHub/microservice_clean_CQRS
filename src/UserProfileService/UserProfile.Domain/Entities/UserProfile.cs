using AbstractionBlocks.Common.Domain;
using UserProfileService.Domain.ValueObjects;
using UserProfileService.Domain.Events;
using System.Text.Json.Serialization;
namespace UserProfileService.Domain.Entities
{
    public class UserProfile : Entity, IAggregateRoot
    {
        public Guid UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Address? Address { get; set; }
        private List<IEventDomain> _events = new List<IEventDomain>();
        [JsonIgnore]
        public IReadOnlyList<IEventDomain> Events => _events;
        public void AddDomainEvent(IEventDomain eventItem)
        {
            _events.Add(eventItem);
        }
        public void RemoveDomainEvent(IEventDomain eventItem)
        {
            _events.Remove(eventItem);
        }
        public void ClearDomainEvents()
        {
            _events.Clear();
        }
        public static UserProfile Create(Guid userId, string firstName, string lastName, string email, string? phoneNumber = null, Address? address = null, Guid? createdBy = null)
        {
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                CreatedAt = DateTime.UtcNow,
                CreateById = createdBy ?? Guid.Empty
            };
            profile.AddDomainEvent(new UserProfileCreatedEvent(profile));
            
            // Raise integration event
            profile.AddDomainEvent(new UserProfileCreatedIntegrationEvent(
                profile.Id,
                profile.UserId,
                profile.FirstName,
                profile.LastName,
                profile.Email,
                profile.PhoneNumber));
            
            return profile;
        }

        public void UpdateProfile(string firstName, string lastName, string email, string? phoneNumber, Address? address, Guid? updatedBy = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedBy;

            // Raise integration event
            AddDomainEvent(new UserProfileUpdatedIntegrationEvent(
                Id,
                UserId,
                FirstName,
                LastName,
                Email,
                PhoneNumber));
        }

        public void Delete(Guid? deletedBy = null)
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = deletedBy;

            // Raise integration event
            AddDomainEvent(new UserProfileDeletedIntegrationEvent(
                Id,
                UserId));
        }

        public UserProfile() { }
    }
}
