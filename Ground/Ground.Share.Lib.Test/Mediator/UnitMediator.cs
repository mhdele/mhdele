using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection;
using Ground.Share.Env;
using Ground.Share.Lib.Mediator;
using Ground.Share.Store;
using LamLibAllOver;

namespace Ground.Share.Lib.Test.Mediator;

public class UnitMediator {
    private static Lib.Mediator.Mediator MediatorFactory() {
        Func<Task<SResult<FileStore>>> fileStoreBuilder = async () => SResult<FileStore>.Ok(new FileStoreEmpty());
        Func<Task<SResult<IDbConnection>>> sqlStoreBuilder = async () => SResult<IDbConnection>.Ok(new DbConnectionEmpty());
        
        var mediator = new Lib.Mediator.Mediator(fileStoreBuilder, sqlStoreBuilder);
        return mediator;
    }
    
    [Fact]
    public void TestNewMediator() {
        MediatorFactory();
    }
    
    [Fact]
    public async Task TestRequestFirst() {
        var mediator = MediatorFactory();
        
        mediator.OverwriteRequests(
            (new Dictionary<Type, IReadOnlyList<Type>>() {
                { typeof(Ping), new Type[] { typeof(SimpleRequestHandler) } }
            }).ToImmutableDictionary()
        );
        Assert.IsType<Lib.Mediator.Mediator>(mediator);
        var requestResult = await mediator.RequestFirstAsync<Ping, String>(new Ping("Hallo2"));
        
        Assert.Equal(EResult.Ok,requestResult.Unwrap().Status);
        Assert.Equal("Hallo2", requestResult.Ok());
    }
    
    [Fact]
    public async Task TestRequestFirstOrDefault() {
        var mediator = MediatorFactory();
        
        mediator.OverwriteRequests(
            (new Dictionary<Type, IReadOnlyList<Type>>() {
                { typeof(Ping), new Type[] { typeof(SimpleRequestHandler) } }
            }).ToImmutableDictionary()
        );
        var requestResultOption = await mediator.RequestFirstOrDefaultAsync<Ping, String>(new Ping("Hallo2"));
        Assert.True(requestResultOption.IsSet(), "requestResultOption.IsSet()");
        var requestResult = requestResultOption.Unwrap();
        Assert.Equal(EResult.Ok,requestResult.Unwrap().Status);
        Assert.Equal("Hallo2", requestResult.Ok());
    }
    
    [Fact]
    public async Task TestNotification() {
        var mediator = MediatorFactory();
        
        mediator.OverwriteNotifications(
            (new Dictionary<Type, IReadOnlyList<Type>>() {
                { typeof(Pong), new Type[] { typeof(SimpleNotificationHandler) } }
            }).ToImmutableDictionary()
        );
        var requestResult = await mediator.NotificationFirstAsync(new Pong());
        
        Assert.Equal(EResult.Err,requestResult.Unwrap().Status);
        Assert.Equal("Pong", requestResult.Err());
    }
    
    [Fact]
    public async Task TestNotificationOrDefault() {
        var mediator = MediatorFactory();
        
        mediator.OverwriteNotifications(
            (new Dictionary<Type, IReadOnlyList<Type>>() {
                { typeof(Pong), new Type[] { typeof(SimpleNotificationHandler) } }
            }).ToImmutableDictionary()
        );
        var requestResult = await mediator.NotificationFirstOrDefaultAsync(new Pong());
        
        Assert.True(requestResult.IsSet());
        Assert.Equal(EResult.Err,requestResult.Unwrap().Unwrap().Status);
        Assert.Equal("Pong", requestResult.Unwrap().Err());
    }
    
    public class SimpleRequestHandler : RequestHandler<Ping, string> {
        public SimpleRequestHandler(Guid lineId, GlobalEnv env, Store.Store store, Lib.Mediator.Mediator mediator) 
            : base(lineId, env, store, mediator) {
        }

        public override Task<SResult<string>> HandleAsync(Ping prop) {
            return Task.FromResult(SResult<string>.Ok(prop.Message));
        }
    }
    
    public class SimpleNotificationHandler: NotificationHandler<Pong> {
        public SimpleNotificationHandler(Guid lineId, GlobalEnv env, Store.Store store, Lib.Mediator.Mediator mediator) 
            : base(lineId, env, store, mediator) {
        }

        public override async Task<SResultErr> HandleAsync(INotification<Pong> prop) {
            return SResultErr.Err("Pong");
        }
    }
    
    public class FileStoreEmpty: FileStore {
        public override ValueTask DisposeAsync() {
            return ValueTask.CompletedTask;
        }
    }

    public class DbConnectionEmpty : IDbConnection {
        private string _connectionString;
        private int _connectionTimeout;
        private string _database;
        private ConnectionState _state;

        public void Dispose() {
        }

        public IDbTransaction BeginTransaction() => default;

        public IDbTransaction BeginTransaction(IsolationLevel il) => default;

        public void ChangeDatabase(string databaseName) { }

        public void Close() { }

        public IDbCommand CreateCommand() => default;

        public void Open() { }

        public string ConnectionString {
            get => _connectionString;
            set => _connectionString = value;
        }

        public int ConnectionTimeout => _connectionTimeout;

        public string Database => _database;

        public ConnectionState State => _state;
    }

    public record Ping(string Message) : IRequest<Ping, string>;

    public record Pong : INotification<Pong>;
}