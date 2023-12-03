using Microsoft.Extensions.Configuration;

namespace Ground.Share.Lib;

public static class Env {
    public static readonly string NpsqlAddress = GetEnvironmentVariableCheckNull("NPSQL_Address");
    public static readonly int    NpsqlPort = Int32.Parse(GetEnvironmentVariableCheckNull("NPSQL_PORT"));
    public static readonly string NpsqlUsername = GetEnvironmentVariableCheckNull("NPSQL_USERNAME");
    public static readonly string NpsqlPassword = GetEnvironmentVariableCheckNull("NPSQL_PASSWORD");
    public static readonly string NpsqlKeyspace = GetEnvironmentVariableCheckNull("NPSQL_KEYSPACE");
    
    
    private static IConfigurationRoot? _config = null;

    private static IConfigurationRoot getConfig() {
        if (_config is not null) return _config;
        _config = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", true)
                  .AddEnvironmentVariables()
                  .Build();
        return getConfig();
    }
    
    private static string GetEnvironmentVariableCheckNull(string name) {
        var config = getConfig();
        return config[name] ?? throw new NullReferenceException("EnvironmentVariable " + name);
    }
}