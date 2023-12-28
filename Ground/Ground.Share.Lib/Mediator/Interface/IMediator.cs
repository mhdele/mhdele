using System.Collections.Immutable;
using System.Data;
using System.Reflection;
using Ground.Share.Env;
using Ground.Share.Store;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator.Interface;

public interface IMediator {
    public Task<SResult<TOutput>> RequestFirstAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop)
        where TInput : IRequest<TInput, TOutput>;

    public Task<Option<SResult<TOutput>>> RequestFirstOrDefaultAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop) where TInput : IRequest<TInput, TOutput>;

    public Task<List<SResult<TOutput>>> RequestsAsync
        <TInput, TOutput>(IRequest<TInput, TOutput> prop) 
        where TInput : IRequest<TInput, TOutput>;
    
    public Task<SResultErr> NotificationFirstAsync<TInput>(TInput prop)
        where TInput: INotification<TInput>;

    public Task<Option<SResultErr>> NotificationFirstOrDefaultAsync<TInput>(TInput prop)
        where TInput: INotification<TInput>;

    public Task<SResultErr> NotificationsAsync<TInput>(TInput prop)
        where TInput: INotification<TInput>;
    
    public IMediator AsMediator() => this;
}