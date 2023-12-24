using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using LamLibAllOver;
using LamLibAllOver.Attributes;

namespace Ground.Share.Lib.Mediator;

internal class MediatorSingle: ICloneable {
    private ImmutableDictionary<Type, IReadOnlyList<Type>> _mediatorTaskDic;
    private readonly Type[] _constructorTypes;
    private readonly Func<object[]> _constructorObjectArrBuilder;
    
    public MediatorSingle(
        ImmutableDictionary<Type, IReadOnlyList<Type>> mediatorTaskDic, 
        IReadOnlyList<Type> constructorTypes, 
        Func<object[]> constructorObjectArrBuilder) {
        _mediatorTaskDic = mediatorTaskDic;
        _constructorTypes = constructorTypes.ToArray();
        _constructorObjectArrBuilder = constructorObjectArrBuilder;
    }

    public async Task<Option<IEResult>> CallHandlerFirstOrDefaultAsync<TInput>(TInput prop) where TInput: IHint {
        if (_mediatorTaskDic.TryGetValue(prop.GetType(), out var list) && list.Count != 0) {
            var handlerTask = CreateHandlerTask(list[0]);
            if (handlerTask == EResult.Err) return Option<IEResult>.With(handlerTask);
            return Option<IEResult>.With(await InvokeHandlerAsync(prop, handlerTask.Ok()));
        }   
        
        return Option<IEResult>.Empty;
    } 
    
    public async Task<IEResult> CallHandlerFirstAsync<TInput>(TInput prop) where TInput: IHint {
        if (_mediatorTaskDic.TryGetValue(prop.GetType(), out var list) && list.Count != 0) {
            var handlerTask = CreateHandlerTask(list[0]);
            if (handlerTask == EResult.Err) return handlerTask;
            return await InvokeHandlerAsync(prop, handlerTask.Ok());
        }   
        
        return SResult<MediatorHandler>.Err(TraceMsg.WithMessage("Mediator Find No Task"));
    }
    
    public async Task<List<IEResult>> CallHandlersAsync<TInput>(TInput prop) where TInput: IHint {
        if (!_mediatorTaskDic.TryGetValue(prop.GetType(), out var list) || list.Count == 0) {
            return [];
        }

        List<IEResult> returnList = new(list.Count);

        foreach (var type in list) {
            var ieResult = await CreateHandlerTask(type).MapAsync(x => InvokeHandlerAsync(prop, x));
            returnList.Add(ieResult);
            if (ieResult == EResult.Err) {
                return returnList;
            }
        }

        return returnList;
    }

    public void AddTask(Type taskType, Type inputType) {
        var dic = _mediatorTaskDic.ToDictionary();
        if (!dic.TryGetValue(inputType, out var list)) {
            list = new List<Type>(1);
            dic.Add(inputType, list);
        }
        else {
            dic[inputType] = list.Append(taskType).AsList();
        }
        _mediatorTaskDic = dic.ToImmutableDictionary();
    }

    public void Clear() => _mediatorTaskDic = ImmutableDictionary<Type, IReadOnlyList<Type>>.Empty;

    public void OverwriteHandlers(ImmutableDictionary<Type, IReadOnlyList<Type>> mediatorTasks) {
        _mediatorTaskDic = mediatorTasks;
    }
    
    private async Task<IEResult> InvokeHandlerAsync<TInput>(
        TInput prop, 
        MediatorHandler handler) where TInput: IHint {
        
        try {
            if (prop is null) {
                return SResultErr.Err(TraceMsg.WithMessage("prop is null"));
            }
            var ieResult = await handler.HandleAsync(prop);
            return ieResult;
        }
        catch (Exception e) {
            return SResultErr.Err(e);
        }
    }

    public SResult<MediatorHandler> CreateHandlerTask(Type type) {
        try {
            var ctor = type.GetConstructor(_constructorTypes);
            if (ctor is null) {
                return SResult<MediatorHandler>.Err(TraceMsg.WithMessage("ConstructorInfo is null"));
            }

            var task = (MediatorHandler)ctor.Invoke(_constructorObjectArrBuilder());
            return SResult<MediatorHandler>.Ok(task);
        }
        catch (Exception e) {
            return SResult<MediatorHandler>.Err(e);
        }  
    }

    public object Clone() {
        return new MediatorSingle(_mediatorTaskDic, _constructorTypes, _constructorObjectArrBuilder);
    }
}