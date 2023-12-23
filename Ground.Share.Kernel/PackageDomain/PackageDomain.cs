using Generator.Equals;

namespace Ground.Share.Kernel.PackageDomain;

[Equatable]
public partial class PackageDomain: Model.ValueObject {
    public PackageBundleDomain BundleDomain { get; }
    public string Domain { get; }
    public Version Version { get; }
    [Generator.Equals.IgnoreEquality]
    public string FullDomain { get; }
    
    public PackageDomain(PackageBundleDomain bundleDomain, string domain, Version version) {
        BundleDomain = bundleDomain;
        Domain = domain;
        Version = version;
        FullDomain = $"{bundleDomain.BundleDomain}.{domain}";
    }
    
    public override IEnumerator<object> GetEnumerator() {
        yield return BundleDomain;
        yield return Domain;
        yield return Version;
        yield return FullDomain;
    }

    public override object Clone() {
        return new PackageDomain(BundleDomain, Domain, Version);
    }
}