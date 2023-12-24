using System.Collections.Immutable;
using System.Data;
using System.Reflection;
using Ground.Share.Env;
using Ground.Share.Store;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public sealed class Mediator {
    private MediatorSingle _mediatorSingleRequest;
    private MediatorSingle _mediatorSingleNotification;
    
    private static readonly System.Type[] RequestTypeConstructor = [typeof(Guid), typeof(GlobalEnv), typeof(Store.Store), typeof(Mediator)]; 
    private static readonly System.Type[] NotificationTypeConstructor = [typeof(Guid), typeof(GlobalEnv), typeof(Store.Store), typeof(Mediator)];
    private Store.Store _store;
    
    public Mediator(
        Func<Task<SResult<FileStore>>> fileStoreBuilder,
        Func<Task<SResult<IDbConnection>>> sqlStoreBuilder
        ) {

        _store = new Store.Store(fileStoreBuilder, sqlStoreBuilder);
        
        Func<object[]> constructorObjectArrBuilderRequest = () => {
            return [
                Guid.NewGuid(),
                GlobalEnv.GetGlobalInstance(),
                _store,
                this        
            ];
        };
        
        Func<object[]> constructorObjectArrBuilderNotification = () => {
            return [
                Guid.NewGuid(),
                GlobalEnv.GetGlobalInstance(),
                _store.CreateNewStore(),
                this        
            ];
        };
        
        _mediatorSingleRequest = new MediatorSingle(
            ImmutableDictionary<Type, IReadOnlyList<Type>>.Empty,
            RequestTypeConstructor,
            constructorObjectArrBuilderRequest
        );
        
        _mediatorSingleNotification = new MediatorSingle(
            ImmutableDictionary<Type, IReadOnlyList<Type>>.Empty,
            NotificationTypeConstructor,
            constructorObjectArrBuilderNotification
        );
    }
    
    public void OverwriteRequests(ImmutableDictionary<Type, IReadOnlyList<Type>> requests) 
        => this._mediatorSingleRequest.OverwriteHandlers(requests);

    public void OverwriteNotifications(ImmutableDictionary<Type, IReadOnlyList<Type>> notifications)
        => this._mediatorSingleNotification.OverwriteHandlers(notifications);

    public async Task<SResult<TOutput>> RequestFirstAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop) {
        IEResult ier = await this._mediatorSingleRequest.CallHandlerFirstAsync(prop);
        if (ier is SResultErr @err) {
            if (err == EResult.Err) {
                return err.ConvertTo<TOutput>();
            }
            return SResult<TOutput>.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResult<TOutput>"));
        }

        if (ier is SResult<TOutput> result) {
            return result;
        }
        
        return SResult<TOutput>.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResult<TOutput>"));
    }
    
    public async Task<Option<SResult<TOutput>>> RequestFirstOrDefaultAsync<TInput, TOutput>(IRequest<TInput, TOutput> prop) {
        Option<IEResult> ierOption = await this._mediatorSingleRequest.CallHandlerFirstOrDefaultAsync(prop);

        return ierOption.Map((ier) => {
            if (ier is SResultErr @err) {
                if (err == EResult.Err) {
                    return err.ConvertTo<TOutput>();
                }
                return SResult<TOutput>.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResult<TOutput>"));
            }

            if (ier is SResult<TOutput> @result) {
                return @result;
            }
        
            return SResult<TOutput>.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResult<TOutput>"));
        });
    }
    
    public async Task<SResultErr> NotificationFirstAsync<TInput>(INotification<TInput> prop) {
        var ier = await this._mediatorSingleNotification.CallHandlerFirstAsync(prop);
        if (ier is SResultErr @err) {
            return @err;
        }

        return SResultErr.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResultErr"));
    } 
    
    public async Task<Option<SResultErr>> NotificationFirstOrDefaultAsync<TInput>(INotification<TInput> prop) {
        var callHandler = (await _mediatorSingleNotification.CallHandlerFirstOrDefaultAsync(prop));
        if (callHandler.IsNotSet()) return Option<SResultErr>.Empty;
        return Option<SResultErr>.With(callHandler.Unwrap() switch {
            SResultErr @err => @err,
            _ => SResultErr.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResultErr"))
        });
    } 
}