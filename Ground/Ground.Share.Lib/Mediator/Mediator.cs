using System.Collections.Immutable;
using System.Data;
using Ground.Share.Env;
using Ground.Share.Store;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public partial class Mediator: IAsyncDisposable, IDisposable {
    private readonly MediatorDictionaryHolder _mediatorDictionaryHolderRequest;
    private readonly MediatorDictionaryHolder _mediatorDictionaryHolderNotification;
    private readonly Func<Task<SResult<FileStore>>> _fileStoreBuilder;
    private readonly Func<Task<SResult<IDbConnection>>> _sqlStoreBuilder;
    private readonly List<Func<Task<SResultErr>>> _triggerAfterDisposable = new();
    private readonly MediatorState _state;
    protected internal MediatorState State => _state;
    
    public Mediator(
        Func<Task<SResult<FileStore>>> fileStoreBuilder,
        Func<Task<SResult<IDbConnection>>> sqlStoreBuilder) {
        
        _fileStoreBuilder = fileStoreBuilder;
        _sqlStoreBuilder = sqlStoreBuilder;
        _mediatorDictionaryHolderRequest = new (ImmutableDictionary<Type, IReadOnlyList<Type>>.Empty);
        _mediatorDictionaryHolderNotification = new (ImmutableDictionary<Type, IReadOnlyList<Type>>.Empty);
        
        _state = StateBuilder();
    }
    
    private MediatorState StateBuilder() => new MediatorState(
        new Store.Store(_fileStoreBuilder, _sqlStoreBuilder), 
        Guid.NewGuid(),
        true,
        GlobalEnv.GetGlobalInstance()
    );
    
    public void OverwriteRequests(ImmutableDictionary<Type, IReadOnlyList<Type>> requests) 
        => this._mediatorDictionaryHolderRequest.OverwriteHandlers(requests);

    public void OverwriteNotifications(ImmutableDictionary<Type, IReadOnlyList<Type>> notifications)
        => this._mediatorDictionaryHolderNotification.OverwriteHandlers(notifications);
    
    public async Task<Mediator> NewSessionAsync() => new(_fileStoreBuilder, _sqlStoreBuilder);

    public MediatorProxy ToProxy() => new MediatorProxy(this);
    
    public class MediatorState: IDisposable, IAsyncDisposable {
        private readonly bool UseDispose;
        private readonly bool IsDispose = false;
        public readonly Store.Store Store;
        public readonly Guid SessionId;
        public readonly GlobalEnv Env;
        public MediatorState(Store.Store store, Guid sessionId, bool useDispose, GlobalEnv env) {
            Store = store;
            SessionId = sessionId;
        }

        public void Deconstruct() {
            Dispose();
        }

        public async ValueTask DisposeAsync() {
            if (UseDispose == false || IsDispose) {
                return;
            }
            
            await Store.DisposeAsync();
            GC.SuppressFinalize(this);
        }
        
        public void Dispose() {
            if (UseDispose == false || IsDispose) {
                return;
            }
            DisposeAsync().GetAwaiter().GetResult();
        }
    }

    public async ValueTask DisposeAsync() {
        await _state.DisposeAsync();
        foreach (var action in _triggerAfterDisposable) {
            action();
        }
    }

    public void Dispose() {
        DisposeAsync().GetAwaiter().GetResult();
    }
}