using Generator.Equals;

namespace Ground.Share.Kernel.PackageDomain;

[Equatable]
public partial class PackageBundleDomain: Model.ValueObject {
    public string BundleDomain { get; }

    public PackageBundleDomain(string bundleDomain) {
        BundleDomain = bundleDomain;
    }

    public override IEnumerator<object> GetEnumerator() {
        yield return BundleDomain;
    }

    public override object Clone() {
        return new PackageBundleDomain(BundleDomain);
    }
}