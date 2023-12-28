using Ground.Share.Env;
using LamLibAllOver;

namespace Ground.Share.Lib.Mediator;

public abstract class NotificationHandler<TInput> 
    : MediatorHandler 
    where TInput: INotification<TInput> {
    public NotificationHandler(MediatorProxy mediator) : base(mediator) { }
    
    public abstract Task<SResultErr> HandleAsync(TInput prop);
    
    public override async Task<IEResult> HandleAsObjectAsync(object prop) {
        if (prop.GetType() != typeof(TInput)) {
            return SResultErr.Err(TraceMsg.WithMessage($"prop has false Type. It Must Be {typeof(TInput)}"));
        }

        return await HandleAsync((TInput) prop);
    }
}