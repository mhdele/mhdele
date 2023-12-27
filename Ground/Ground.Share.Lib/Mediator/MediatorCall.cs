using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public partial class Mediator : IMediator {
    public async Task<SResult<TOutput>> RequestFirstAsync
        <TInput, TOutput>(IRequest<TInput, TOutput> prop)
        where TInput : IRequest<TInput, TOutput> {
        try {
            SResult<Type> typeResult = this._mediatorDictionaryHolderRequest.GetTypeHandlerFirst(prop);
            if (typeResult == EResult.Err) return typeResult.ChangeOkType<TOutput>();

            var type = typeResult.Ok();
            return await ExecuteRequestAsync(type, prop);
        }
        catch (Exception e) {
            return SResult<TOutput>.Err(e);
        }
    }

    public async Task<Option<SResult<TOutput>>> RequestFirstOrDefaultAsync
        <TInput, TOutput>(IRequest<TInput, TOutput> prop)
        where TInput : IRequest<TInput, TOutput> {
        var typeResultOption = this._mediatorDictionaryHolderRequest.CallHandlerFirstOrDefaultAsync(prop);

        if (typeResultOption.IsNotSet()) return Option<SResult<TOutput>>.Empty;

        try {
            Type type = typeResultOption.Unwrap();
            return Option<SResult<TOutput>>.With(await ExecuteRequestAsync(type, prop));
        }
        catch (Exception e) {
            return Option<SResult<TOutput>>.With(SResult<TOutput>.Err(e));
        }
    }

    public async Task<List<SResult<TOutput>>> RequestsAsync
        <TInput, TOutput>(IRequest<TInput, TOutput> prop) 
        where TInput : IRequest<TInput, TOutput>{
        
        try {
            SResult<List<Type>> typeResult = this._mediatorDictionaryHolderRequest.GetTypeHandlers(prop);
            if (typeResult == EResult.Err) return new () { typeResult.ChangeOkType<TOutput>() };

            List<SResult<TOutput>> results = new(typeResult.Ok().Count);
            foreach (var type in typeResult.Ok()) {
                var result = await ExecuteRequestAsync<TInput, TOutput>(type, prop);
                results.Add(result);
                if (result == EResult.Err) return results;
            }

            return results;
        }
        catch (Exception e) {
            return new List<SResult<TOutput>>() { SResult<TOutput>.Err(e) };
        }
    }
    
    public async Task<SResultErr> NotificationFirstAsync<TInput>(INotification<TInput> prop) {
        try {
            SResult<Type> typeResult = this._mediatorDictionaryHolderNotification.GetTypeHandlerFirst(prop);
            if (typeResult == EResult.Err) return typeResult;

            var type = typeResult.Ok();
            return await AddNotificationToQueueAsync(type, prop);
        }
        catch (Exception e) {
            return SResultErr.Err(e);
        }

        return SResultErr.Err(TraceMsg.WithMessage("IEResult Has False Type; Must be SResultErr"));
    }

    public async Task<Option<SResultErr>> NotificationFirstOrDefaultAsync<TInput>(INotification<TInput> prop) {
        try {
            var callType = _mediatorDictionaryHolderNotification.CallHandlerFirstOrDefaultAsync(prop);
            return callType.IsNotSet()
                ? Option<SResultErr>.Empty
                : Option<SResultErr>.With(await AddNotificationToQueueAsync(callType.Unwrap(), prop));
        }
        catch (Exception e) {
            return Option<SResultErr>.With(SResultErr.Err(e));
        }
    }
    
    public async Task<SResultErr> NotificationsAsync<TInput>(INotification<TInput> prop) {
        try {
            var callTypes = _mediatorDictionaryHolderNotification.GetTypeHandlers(prop);
            if (callTypes == EResult.Err) return callTypes;

            foreach (var type in callTypes.Ok()) {
                await AddNotificationToQueueAsync(type, prop);
            }

            return SResultErr.Ok();
        }
        catch (Exception e) {
            return SResultErr.Err(e);
        }
    }

    private async Task<SResultErr> AddNotificationToQueueAsync<TInput>(Type type, INotification<TInput> prop) {
        try {
            if (typeof(NotificationHandler<INotification<TInput>>) != type) {
                return SResultErr.Err(TraceMsg.WithMessage(
                    $"Type Has False Type: Must Be {typeof(NotificationHandler<INotification<TInput>>)}"));
            }

            Mediator mediator = await this.NewSessionAsync();
            MediatorProxy mediatorProxy = new MediatorProxy(mediator);
            this._triggerAfterDisposable.Add(async () => {
                await using (mediator) {
                    try {
                        var handlerResult = CreateHandler(mediatorProxy, type);
                        if (handlerResult == EResult.Err) return handlerResult;

                        var handler = handlerResult.Ok();
                        var handlerResultValue = (SResultErr)await handler.HandleAsObjectAsync(prop);

                        return handlerResultValue;
                    }
                    catch (Exception e) {
                        return SResultErr.Err(e);
                    }
                }
            });

            return SResultErr.Ok();
        }
        catch (Exception e) {
            return SResultErr.Err(e);
        }
    }

    private async Task<SResult<TOutput>> ExecuteRequestAsync<TInput, TOutput>(
        Type type,
        IRequest<TInput, TOutput> prop)
        where TInput : IRequest<TInput, TOutput> {
        try {
            if (!RightType<TInput, TOutput>(type, out var resultErr)) return resultErr.ConvertTo<TOutput>();

            var handlerResult = CreateHandler(new MediatorProxy(this), type);
            if (handlerResult == EResult.Err) return handlerResult.ChangeOkType<TOutput>();

            var handler = (RequestHandler<TInput, TOutput>)handlerResult.Ok();
            var handlerResultValue = await handler.HandleAsync((TInput)prop);
            return handlerResultValue;
        }
        catch (Exception e) {
            return SResult<TOutput>.Err(e);
        }
    }

    private static bool RightType<TInput, TOutput>(Type type, out SResultErr resultErr)
        where TInput : IRequest<TInput, TOutput> {
        if (type != typeof(RequestHandler<TInput, TOutput>)) {
            resultErr = SResultErr.Err(
                TraceMsg.WithMessage($"Type Has False Type Must Be {typeof(RequestHandler<TInput, TOutput>)}")
            );
            return false;
        }

        resultErr = SResultErr.Ok();
        return true;
    }

    private SResult<MediatorHandler> CreateHandler(MediatorProxy proxy, Type type) {
        try {
            var ctor = type.GetConstructor([typeof(MediatorProxy)]);
            if (ctor is null) {
                return SResult<MediatorHandler>.Err(TraceMsg.WithMessage("ConstructorInfo is null"));
            }

            return SResult<MediatorHandler>.Ok((MediatorHandler)ctor.Invoke([proxy]));
        }
        catch (Exception e) {
            return SResult<MediatorHandler>.Err(e);
        }
    }
}