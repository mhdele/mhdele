using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using LamLibAllOver;
using LamLibAllOver.Attributes;

namespace Ground.Share.Lib.Mediator;

internal sealed class MediatorDictionaryHolder: ICloneable {
    private ImmutableDictionary<Type, IReadOnlyList<Type>> _mediatorTaskDic;
    
    public MediatorDictionaryHolder(ImmutableDictionary<Type, IReadOnlyList<Type>> mediatorTaskDic) 
        => _mediatorTaskDic = mediatorTaskDic;

    public Option<Type> CallHandlerFirstOrDefaultAsync<TInput>(TInput prop) where TInput: IHint {
        if (_mediatorTaskDic.TryGetValue(prop.GetType(), out var list) && list.Count != 0) {
            return Option<Type>.With(list[0]);
        }   
        
        return Option<Type>.Empty;
    } 
    
    public SResult<Type> GetTypeHandlerFirst<TInput>(TInput prop) where TInput: IHint {
        if (_mediatorTaskDic.TryGetValue(prop.GetType(), out IReadOnlyList<Type>? list) && list.Count != 0) {
            return SResult<Type>.Ok(list[0]);
        }   
        
        return SResult<Type>.Err(TraceMsg.WithMessage("Mediator Find No Task"));
    }
    
    public SResult<List<Type>> GetTypeHandlers<TInput>(TInput prop) where TInput: IHint {
        if (_mediatorTaskDic.TryGetValue(prop.GetType(), out IReadOnlyList<Type>? list) && list.Count != 0) {
            return SResult<List<Type>>.Ok(list.ToList());
        }   
        
        return SResult<List<Type>>.Err(TraceMsg.WithMessage("Mediator Find No Task"));
    }
    
    public List<Type> GetTypeHandlersAsync<TInput>(bool sameState, TInput prop) where TInput: IHint {
        if (!_mediatorTaskDic.TryGetValue(prop.GetType(), out var list) || list.Count == 0) {
            return [];
        }

        return list.ToList();
    }

    public void AddHandler(Type taskType, Type inputType) {
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

    public object Clone() {
        return new MediatorDictionaryHolder(_mediatorTaskDic);
    }
}