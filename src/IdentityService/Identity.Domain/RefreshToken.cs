namespace IdentityService.Identity.Domain
{
    public class RefreshToken
    {
        public Guid Token { get; private set; }
        public DateTime Expiry { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private RefreshToken(Guid token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry;
        }
        public void Revoke() => IsRevoked = true;
        public static RefreshToken Create(DateTime expiry) => new RefreshToken(Guid.NewGuid(), expiry);
    }
}
