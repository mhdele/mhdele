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
    public async Task TestRequests() {
        var mediator = MediatorFactory();
        
        mediator.OverwriteRequests(
            (new Dictionary<Type, IReadOnlyList<Type>>() {
                { typeof(Ping), new Type[] { typeof(SimpleRequestHandler), typeof(SimpleRequestHandler2) } },
            }).ToImmutableDictionary()
        );
        var requestResultOptions = await mediator.RequestsAsync<Ping, String>(new Ping("Hallo2"));
        foreach (var requestResult in requestResultOptions) {
            Assert.Equal(EResult.Ok,requestResult.Unwrap().Status);
            Assert.Equal("Hallo2", requestResult.Ok());    
        }
    }
    
    [Fact]
    public async Task TestNotification() {
        string resText = "NULL";
        
        var v = () => {
            resText = "Pong";
        };

        {
            await using var mediator = MediatorFactory();
            mediator.OverwriteNotifications(
                (new Dictionary<Type, IReadOnlyList<Type>>() {
                    { typeof(Pong), new Type[] { typeof(SimpleNotificationHandler) } }
                }).ToImmutableDictionary()
            );
            SResultErr requestResult = await mediator.NotificationFirstAsync(new Pong(v));
            Assert.Equal(EResult.Ok,requestResult.Unwrap().Status);
        }
        
        
        Assert.Equal("Pong", resText);
    }
    
    [Fact]
    public async Task TestNotifications() {
        string resText = "";
        
        var v = () => { resText += "Pong"; };

        {
            await using var mediator = MediatorFactory();
            mediator.OverwriteNotifications(
                (new Dictionary<Type, IReadOnlyList<Type>>() {
                    { typeof(Pong), new Type[] { typeof(SimpleNotificationHandler), typeof(SimpleNotificationHandler2) } }
                }).ToImmutableDictionary()
            );
            SResultErr requestResult = await mediator.NotificationsAsync(new Pong(v));
            Assert.Equal(EResult.Ok,requestResult.Unwrap().Status);
        }
        
        
        Assert.Equal("PongPong", resText);
    }
    
    [Fact]
    public async Task TestNotificationOrDefault() {
        
        string resText = "NULL";
        
        var v = () => {
            resText = "Pong";
        };


        {
            await using var mediator = MediatorFactory();
            mediator.OverwriteNotifications(
                (new Dictionary<Type, IReadOnlyList<Type>>() {
                    { typeof(Pong), new Type[] { typeof(SimpleNotificationHandler) } }
                }).ToImmutableDictionary()
            );
            var requestResult = await mediator.NotificationFirstOrDefaultAsync(new Pong(v));
            Assert.True(requestResult.IsSet());
            Assert.Equal(EResult.Ok,requestResult.Unwrap().Unwrap().Status);
        }
        
        Assert.Equal("Pong", resText);
    }
    
    public class SimpleRequestHandler : RequestHandler<Ping, string> {
        public SimpleRequestHandler(MediatorProxy mediatorProxy) : base(mediatorProxy) { }

        public override Task<SResult<string>> HandleAsync(Ping prop) {
            return Task.FromResult(SResult<string>.Ok(prop.Message));
        }
    }
    
    public class SimpleNotificationHandler: NotificationHandler<Pong> {
        public SimpleNotificationHandler(MediatorProxy mediatorProxy) : base(mediatorProxy) {
        }

        public override async Task<SResultErr> HandleAsync(Pong prop) {
            prop.Action();
            return SResultErr.Ok();
        }
    }
    
    public class SimpleRequestHandler2 : RequestHandler<Ping, string> {
        public SimpleRequestHandler2(MediatorProxy mediatorProxy) : base(mediatorProxy) { }

        public override Task<SResult<string>> HandleAsync(Ping prop) {
            return Task.FromResult(SResult<string>.Ok(prop.Message));
        }
    }
    
    public class SimpleNotificationHandler2: NotificationHandler<Pong> {
        public SimpleNotificationHandler2(MediatorProxy mediatorProxy) : base(mediatorProxy) {
        }

        public override async Task<SResultErr> HandleAsync(Pong prop) {
            prop.Action();
            return SResultErr.Ok();
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

    public record Pong(Action Action) : INotification<Pong>;
}