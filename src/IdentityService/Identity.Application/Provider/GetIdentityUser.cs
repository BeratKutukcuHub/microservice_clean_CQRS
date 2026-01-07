namespace IdentityService.Identity.Application.Provider
{
    public class GetIdentityUser
    {
        public string Id { get; set; }
        public string Email { get; set; } 
        public List<Guid> RoleIds { get; set; } 
        public bool IsBlocked { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}