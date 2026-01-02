namespace IdentityService.Application.Auth.Identity.Profile
{
    public record UpdateIdentityResponse(Guid id, string name, string email);
    public record IdentityUserDto(Guid id, string name, string email, IEnumerable<string> roles,
    DateTime createdAt) : UpdateIdentityResponse(id, name, email);
}
