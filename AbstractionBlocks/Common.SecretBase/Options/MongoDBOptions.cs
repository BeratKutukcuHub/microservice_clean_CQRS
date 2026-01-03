namespace AbstractionBlocks.Common.SecretBase.Options
{
    public sealed class MongoDBOptions : IJsonOption
    {
        public string ConnectionString { get; set; }
    }
} 