using Generator.Equals;

namespace Ground.Share.Kernel.ValueObject;

[Equatable]
[ToString]
public partial class Token: Model.ValueObject {
    public string Value { get; private set; }

    public Token(string value) {
        Value = value;
    }
    
    public override IEnumerator<object> GetEnumerator() {
        yield return Value;
    }

    public override object Clone() => new Token(Value);
}