namespace Ground.Share.Kernel.ValueObject;

public class UserId: Model.ValueObject {
    public long Id { get; private set; }

    public UserId(long id) {
        Id = id;
    }

    internal override IEnumerable<object> GetEquality() {
        yield return Id;
    }
}