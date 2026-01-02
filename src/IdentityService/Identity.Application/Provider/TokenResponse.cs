namespace IdentityService.Application.Provider
{
    public record TokenResponse(string Token, Guid RefreshToken, DateTime createdAt);
    public record LoginResponse(string Token, Guid RefreshToken);
}
