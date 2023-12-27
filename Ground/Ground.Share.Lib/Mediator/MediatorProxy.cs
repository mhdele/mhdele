using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public class MediatorProxy: IMediator {
    private Mediator _mediator;
    public Mediator.MediatorState State => _mediator.State;
    public Store.Store Store => this._mediator.State.Store;
    public Guid SessionId => this._mediator.State.SessionId;
    public GlobalEnv Env => this._mediator.State.Env;
    
    public MediatorProxy(Mediator mediator) {
        _mediator = mediator;
    }

    public Task<SResult<TOutput>> RequestFirstAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop) 
        where TInput : IRequest<TInput, TOutput> {
        
        return _mediator.RequestFirstAsync(prop);
    }

    public Task<Option<SResult<TOutput>>> RequestFirstOrDefaultAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop)
        where TInput : IRequest<TInput, TOutput> {
        
        return _mediator.RequestFirstOrDefaultAsync(prop);
    }

    public Task<List<SResult<TOutput>>> RequestsAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop) where TInput : IRequest<TInput, TOutput> {
        return _mediator.RequestsAsync(prop);
    }

    public Task<SResultErr> NotificationFirstAsync<TInput>(INotification<TInput> prop) {
        return _mediator.NotificationFirstAsync(prop);
    }

    public Task<Option<SResultErr>> NotificationFirstOrDefaultAsync<TInput>(INotification<TInput> prop) {
        return _mediator.NotificationFirstOrDefaultAsync(prop);
    }

    public Task<SResultErr> NotificationsAsync<TInput>(INotification<TInput> prop) {
        return _mediator.NotificationsAsync(prop);
    }
}