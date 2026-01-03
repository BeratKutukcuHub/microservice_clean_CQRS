using System.Text.Json;
using AbstractionBlocks.Common.SecretBase.Options;
using Microsoft.Extensions.Configuration;

namespace AbstractionBlocks.Common.SecretBase.Provider
{
    
    public class SecretProvider<TBind> : ISecretProvider<TBind> where TBind : IJsonOption
    {
        private readonly IConfiguration _localConfig;
    
        // Allow DI to inject IConfiguration (host config) when available
        public SecretProvider(IConfiguration configuration)
        {
            _localConfig = configuration;
        }

        // Fallback for scenarios where IConfiguration isn't provided
        public SecretProvider()
        {
            _localConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("secretbase.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public TBind GetSection()
        {
            var section = _localConfig.GetSection(typeof(TBind).Name);
            if (!section.Exists())
            {
                // Try to locate a secretbase.json file in the directory tree and load it if present
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