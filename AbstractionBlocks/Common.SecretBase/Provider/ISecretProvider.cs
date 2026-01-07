using AbstractionBlocks.Common.SecretBase.Options;
namespace AbstractionBlocks.Common.SecretBase.Provider
{
    public interface ISecretProvider<TBind> where TBind : IJsonOption
    {
        TBind GetSection();
    }
} 