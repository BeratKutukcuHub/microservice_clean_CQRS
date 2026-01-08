using System.Text.Json;
using AbstractionBlocks.Common.SecretBase.Options;
using Microsoft.Extensions.Configuration;
namespace AbstractionBlocks.Common.SecretBase.Provider
{
    public class SecretProvider<TBind> : ISecretProvider<TBind> where TBind : IJsonOption
    {
        private readonly IConfiguration _localConfig;
        public SecretProvider(IConfiguration configuration)
        {
            _localConfig = configuration;
        }
        public SecretProvider()
        {
            _localConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("secretbase.json", optional: true, reloadOnChange: true)
                .Build();
        }
        public TBind GetSection()
        {
            // First try to get from IConfiguration (includes appsettings.json and environment variables)
            var section = _localConfig.GetSection(typeof(TBind).Name);
            
            // If not found, try without "Options" suffix (e.g., "MongoDB" instead of "MongoDBOptions")
            if (!section.Exists())
            {
                var nameWithoutOptions = typeof(TBind).Name.Replace("Options", "");
                section = _localConfig.GetSection(nameWithoutOptions);
            }
            
            // If still not found, try to find secretbase.json file
            if (!section.Exists())
            {
                string? found = null;
                var dir = AppContext.BaseDirectory;
                while (!string.IsNullOrEmpty(dir))
                {
                    var candidate = Path.Combine(dir, "secretbase.json");
                    if (File.Exists(candidate))
                    {
                        found = candidate;
                        break;
                    }
                    var parent = Directory.GetParent(dir);
                    dir = parent?.FullName;
                }
                if (found != null)
                {
                    var fileConfig = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(found)!)
                        .AddJsonFile(Path.GetFileName(found), optional: false, reloadOnChange: false)
                        .Build();
                    section = fileConfig.GetSection(typeof(TBind).Name);
                }
                if (!section.Exists())
                    throw new Exception($"Section '{typeof(TBind).Name}' not found");
            }
            var result = section.Get<TBind>();
            if (result == null)
                throw new Exception($"Section '{typeof(TBind).Name}' could not be bound to {typeof(TBind).Name}");
            return result;
        }
    }
}