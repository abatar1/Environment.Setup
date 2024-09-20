using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Environment.Setup;

// TODO Notify TEnvironmentEntity with updates when Set is called?
internal sealed class EnvironmentEntityProvider(IDictionary<Type, IEnvironmentEntity> environmentEntities)
    : IEnvironmentEntityProvider
{
    private readonly ConcurrentDictionary<Type, IEnvironmentEntity> _environmentEntityMap = new(environmentEntities);

    public TEnvironmentEntity Get<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        return (TEnvironmentEntity)_environmentEntityMap[typeof(TEnvironmentEntity)];
    }
    
    public TEnvironmentEntity Set<TEnvironmentEntity, TValue>(Expression<Func<TEnvironmentEntity, TValue>> entityEnricher, TValue value)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        _environmentEntityMap.AddOrUpdate(typeof(TEnvironmentEntity),
            _ => throw new EnvironmentSetupException($"Could not add new value, {nameof(Set)} could be used for update only."),
            (_, y) => ((TEnvironmentEntity)y).SetupProperty(entityEnricher, value, "CURRENTLY_UNKNOWN"));
        return (TEnvironmentEntity)_environmentEntityMap[typeof(TEnvironmentEntity)];
    }
}