using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class MediatorTask {
    public abstract Task<IEResult> HandleAsync(object prop);    
}