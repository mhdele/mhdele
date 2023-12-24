using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class RequestHandler<TInput, TOutput>: MediatorHandler where TInput: IRequest<TInput, TOutput> {
    public RequestHandler(Guid lineId, GlobalEnv env, Store.Store store, Mediator mediator) : base(lineId, env, store, mediator) {
    }
    
    public abstract Task<SResult<TOutput>> HandleAsync(TInput prop);

    public override async Task<IEResult> HandleAsync(object prop) {
        if (prop.GetType() != typeof(TInput)) {
            return SResult<TOutput>.Err(TraceMsg.WithMessage($"prop has false Type. It Must Be {typeof(TInput)}"));
        }

        return await HandleAsync((TInput)prop);
    }
}