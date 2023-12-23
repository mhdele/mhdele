using Generator.Equals;

namespace Ground.Share.Kernel.ValueObject;

[Equatable]
public partial class UserId: Model.ValueObject {
    public long Id { get; private set; }

    public UserId(long id) {
        Id = id;
    }

    public override IEnumerator<object> GetEnumerator() {
        yield return Id;
    }

    public override object Clone() => new UserId(this.Id);
}