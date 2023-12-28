using Ground.Share.Kernel.PackageDomain;
using Ground.Share.Lib.Package.Enum;
using Ground.Share.Lib.Package.Interface;
using LamLibAllOver;

namespace Ground.Share.Lib.Package;

public interface IPackage {
    public PackageDomain BundleDomain { get; }
    public ELoadLevel Level { get; }
    public EPackageModule Module { get; }

    public Task<SResult<IModule>> ModuleBuildAsync();
}