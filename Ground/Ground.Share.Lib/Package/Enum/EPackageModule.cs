namespace Ground.Share.Lib.Package.Enum;

public enum EPackageModule {
    InternalWeb = 1,
    InternalGrpc = 2,
    InternalEnv = 3,
    InternalStore = 4,
    InternalEmail = 5,
    
    ExternEnv = 101,
    ExternStore = 102,
    ExternEmail = 103,
    ExternAuth = 104,
    ExternSession = 105,
    ExternFrontend = 106,
    ExternElse = 107,
}