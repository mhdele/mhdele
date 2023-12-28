using Ground.Share.Env;
using Ground.Share.Lib.Mediator.Interface;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class MediatorHandler {
    private readonly MediatorProxy _mediatorProxy;
    protected IMediator Mediator => _mediatorProxy;
    protected Guid SessionId => _mediatorProxy.SessionId;
    protected GlobalEnv Env => _mediatorProxy.Env;
    protected Ground.Share.Store.Store Store => _mediatorProxy.Store;
    

    protected MediatorHandler(MediatorProxy mediator) {
        _mediatorProxy = mediator;
    }

    public abstract Task<IEResult> HandleAsObjectAsync(object prop);    
}