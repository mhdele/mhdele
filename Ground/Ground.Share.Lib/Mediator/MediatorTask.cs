using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class MediatorTask {
    protected Guid LineId { get; }
    protected GlobalEnv Env { get; }
    protected Ground.Share.Store.Store Store { get; }

    protected Mediator Mediator { get; }

    protected MediatorTask(Guid lineId, GlobalEnv env, Store.Store store, Mediator mediator) {
        LineId = lineId;
        Env = env;
        Store = store;
        Mediator = mediator;
    }

    public abstract Task<IEResult> HandleAsync(object prop);    
}