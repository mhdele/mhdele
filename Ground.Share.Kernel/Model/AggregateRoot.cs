namespace Ground.Share.Kernel.Model;

public class AggregateRoot<TId> : Entity<TId> {
    internal AggregateRoot(TId id) : base(id) { }
}