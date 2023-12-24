using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class NotificationHandler<TInput>: MediatorHandler {
    public NotificationHandler(Guid lineId, GlobalEnv env, Store.Store store, Mediator mediator) 
        : base(lineId, env, store, mediator) {
    }
    
    public abstract Task<SResultErr> HandleAsync(INotification<TInput> prop);
    
    public override async Task<IEResult> HandleAsync(object prop) {
        if (prop.GetType() != typeof(TInput)) {
            return SResultErr.Err(TraceMsg.WithMessage($"prop has false Type. It Must Be {typeof(TInput)}"));
        }

        return await HandleAsync((INotification<TInput>)prop);
    }
}