using Ground.Share.Kernel;
using Ground.Share.Kernel.PackageDomain;
using LamLibAllOver;

namespace Ground.Share.Lib.Package;

public interface IPackageBundle: IEqualityComparer<IPackageBundle> {
    public Enum.EPackageBundleLoadType LoadType { get; }
    public Version Version { get; }
    public PackageBundleDomain BundleDomain { get; }
    
    public Task<SResult<IPackage[]>> BuildPackages();
}