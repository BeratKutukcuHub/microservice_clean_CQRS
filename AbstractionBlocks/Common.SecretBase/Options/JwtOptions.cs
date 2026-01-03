namespace AbstractionBlocks.Common.SecretBase.Options
{
    public sealed class JwtOptions : IJsonOption
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
} 