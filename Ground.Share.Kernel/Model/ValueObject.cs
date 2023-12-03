namespace Ground.Share.Kernel.Model;

public abstract class ValueObject {
    internal abstract IEnumerable<object> GetEquality();
}