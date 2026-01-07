namespace IdentityService.Application.Auth.Identity.Profile
{
    public record RoleDto(Guid id, string name, IEnumerable<string> permissions);
}
