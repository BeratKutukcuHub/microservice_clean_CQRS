namespace IdentityService.Application.Auth.Identity.Profile
{
    public record UpdateIdentityResponse(Guid id, string name, string email);
    public record IdentityUserDto(Guid id, string name, string email) : UpdateIdentityResponse(id, name, email);
}
