namespace Ground.Share.Store;

public abstract class FileStore: IAsyncDisposable {
    public abstract ValueTask DisposeAsync();
}