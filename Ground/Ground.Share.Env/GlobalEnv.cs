using System.Collections.Immutable;
using LamLibAllOver;

namespace Ground.Share.Env;

public sealed class GlobalEnv {
    public ImmutableDictionary<string, string> ConfigEnv { get; }
    public ImmutableDictionary<string, string> DatabaseEnv { get; }
    
    private GlobalEnv(ImmutableDictionary<string, string> configEnv, ImmutableDictionary<string, string> databaseEnv) {
        ConfigEnv = configEnv;
        DatabaseEnv = databaseEnv;
    }

    private static GlobalEnv Instance = new GlobalEnv(
        new Dictionary<string, string>().ToImmutableDictionary(), 
        new Dictionary<string, string>().ToImmutableDictionary());

    public static GlobalEnv GetGlobalInstance() => Instance;

    public static void SetGlobalInstance(
        ImmutableDictionary<string, string> configEnv,
        ImmutableDictionary<string, string> databaseEnv) {

        Instance = new GlobalEnv(configEnv, databaseEnv);
    }
}