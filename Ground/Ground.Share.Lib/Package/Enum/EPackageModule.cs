namespace Ground.Share.Lib.Package.Enum;

public enum EPackageModule {
    InternalWeb = 1,
    InternalGrpc = 2,
    InternalEnv = 3,
    InternalStore = 4,
    InternalEmail = 5,
    
    PluginEnv = 101,
    PluginStore = 102,
    PluginEmail = 103,
    PluginAuth = 104,
    PluginSession = 105,
    PluginFrontend = 106,
    PluginElse = 107,
}