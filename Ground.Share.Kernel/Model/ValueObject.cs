using System.Collections;

namespace Ground.Share.Kernel.Model;

public abstract class ValueObject: ICloneable {
    public abstract IEnumerator<object> GetEnumerator();
    public abstract object Clone();
}