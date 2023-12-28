namespace Ground.Share.Lib.Interface;

public interface IRepository {
    public Version Version { get; }
    public string DomainName { get; }
}