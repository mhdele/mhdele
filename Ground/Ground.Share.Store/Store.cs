using System.Data;
using LamLibAllOver;

namespace Ground.Share.Store;

public sealed class Store: IAsyncDisposable {
    private readonly Func<Task<SResult<FileStore>>> _fileStoreFn;
    private readonly Func<Task<SResult<IDbConnection>>> _sqlStoreFn;
    
    private Option<FileStore> _fileStore = default;
    private Option<IDbConnection> _sqlStore = default;
    
    public Store(Func<Task<SResult<FileStore>>> fileStoreFn, Func<Task<SResult<IDbConnection>>> sqlStoreFn) {
        _fileStoreFn = fileStoreFn;
        _sqlStoreFn = sqlStoreFn;
    }

    public async Task<SResult<FileStore>> FileStore() {
        try {
            if (_fileStore.IsSet()) {
                return SResult<FileStore>.Ok(_fileStore.Unwrap());
            }

            var storeResult = await _fileStoreFn();
            if (storeResult == EResult.Err) {
                return storeResult.ChangeOkType<FileStore>();
            }

            var store = storeResult.Ok();
            _fileStore = Option<FileStore>.With(store);
            return SResult<FileStore>.Ok(store);
        }
        catch (Exception e) {
            return SResult<FileStore>.Err(e);
        }
    }

    public async Task<SResult<IDbConnection>> DbStore() {
        try {
            if (_fileStore.IsSet()) {
                return SResult<IDbConnection>.Ok(_sqlStore.Unwrap());
            }

            var storeResult = await _sqlStoreFn();
            if (storeResult == EResult.Err) {
                return storeResult.ChangeOkType<IDbConnection>();
            }

            var store = storeResult.Ok();
            _sqlStore = Option<IDbConnection>.With(store);
            return SResult<IDbConnection>.Ok(store);
        }
        catch (Exception e) {
            return SResult<IDbConnection>.Err(e);
        }
    }

    public Store CreateNewStore() {
        return new Store(_fileStoreFn, _sqlStoreFn);
    }

    public void Deconstruct() {
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync() {
        if (_fileStore.IsSet()) {
            await _fileStore.Unwrap().DisposeAsync();
        }
        if (_sqlStore.IsSet()) {
            _sqlStore.Unwrap().Dispose();
        }
        GC.SuppressFinalize(this);
    }
}