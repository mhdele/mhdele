using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class Notification<TInput>: MediatorTask {
    public Guid LineId { get; }
    public GlobalEnv Env { get; }
    public Ground.Share.Store.Store Store { get; }

    protected Notification(Guid lineId, GlobalEnv env, Store.Store store) {
        LineId = lineId;
        Env = env;
        Store = store;
    }
    
    public abstract Task<SResultErr> HandleAsync(TInput prop);
    
    public override async Task<IEResult> HandleAsync(object prop) {
        if (prop.GetType() != typeof(TInput)) {
            return SResultErr.Err(TraceMsg.WithMessage($"prop has false Type. It Must Be {typeof(TInput)}"));
        }

        return await HandleAsync(prop);
    }
}