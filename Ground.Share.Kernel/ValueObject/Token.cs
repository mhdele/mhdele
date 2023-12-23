namespace Ground.Share.Kernel.ValueObject;

[ToString]
public class Token: Model.ValueObject {
    public string Value { get; private set; }

    public Token(string value) {
        Value = value;
    }

    internal override IEnumerable<object> GetEquality() {
        yield return Value;
    }
}