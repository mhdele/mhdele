namespace Ground.Share.Kernel.Model;

public abstract class Entity<TId> 
    where TId : notnull {

    public TId Id { get; protected set; }

    internal Entity(TId id) {
        Id = id;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        
        if (obj.GetType() != this.GetType()) {
            return false;
        }

        return this == (Entity<TId>)obj;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Id.Equals(right.Id);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);

    public override int GetHashCode() => this.Id.GetHashCode();

    public override string ToString() {
        return $"Entity {{ Id = {Id} }}";
    }
}