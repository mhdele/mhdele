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

    public async Task<Option<IEResult>> MediatorTaskFirstOrDefaultAsync<TInput>(TInput prop) {
        if (_mediatorTaskDic.TryGetValue(typeof(TInput), out var list) && list.Count != 0) {
            return Option<IEResult>.With(
                await CreateMediatorTask(list[0]).MapAsync(x => CallMediatorTaskAsync(prop, x))
                );
        }   
        
        return Option<IEResult>.Empty;
    } 
    
    public async Task<IEResult> MediatorTaskFirstAsync<TInput>(TInput prop) {
        if (_mediatorTaskDic.TryGetValue(typeof(TInput), out var list) && list.Count != 0) {
            return await CreateMediatorTask(list[0]).MapAsync(x => CallMediatorTaskAsync(prop, x));
        }   
        
        return SResult<MediatorTask>.Err(TraceMsg.WithMessage("Mediator Find No Task"));
    }
    
    public async Task<List<IEResult>> MediatorTaskAsync<TInput>(TInput prop) {
        if (!_mediatorTaskDic.TryGetValue(typeof(TInput), out var list) || list.Count == 0) {
            return [];
        }

        List<IEResult> returnList = new(list.Count);

        foreach (var type in list) {
            var ieResult = await CreateMediatorTask(type).MapAsync(x => CallMediatorTaskAsync(prop, x));
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

    public void OverwriteMediatorTasks(ImmutableDictionary<Type, IReadOnlyList<Type>> mediatorTasks) {
        _mediatorTaskDic = mediatorTasks;
    }
    
    private async Task<IEResult> CallMediatorTaskAsync<TInput>(
        TInput prop, 
        MediatorTask task) {
        
        try {
            if (prop is null) {
                return SResultErr.Err(TraceMsg.WithMessage("prop is null"));
            }
            return await task.HandleAsync(prop);
        }
        catch (Exception e) {
            return SResultErr.Err(e);
        }
    }

    public SResult<MediatorTask> CreateMediatorTask(Type type) {
        try {
            var ctor = type.GetConstructor(_constructorTypes);
            if (ctor is null) {
                return SResult<MediatorTask>.Err(TraceMsg.WithMessage("ConstructorInfo is null"));
            }

            var task = (MediatorTask)ctor.Invoke(_constructorObjectArrBuilder());
            return SResult<MediatorTask>.Ok(task);
        }
        catch (Exception e) {
            return SResult<MediatorTask>.Err(e);
        }  
    }

    public object Clone() {
        return new MediatorSingle(_mediatorTaskDic, _constructorTypes, _constructorObjectArrBuilder);
    }
}