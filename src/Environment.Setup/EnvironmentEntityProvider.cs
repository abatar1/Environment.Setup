using System;
using System.Collections.Generic;
using System.Linq;

namespace Environment.Setup;

internal sealed class EnvironmentEntityProvider : IEnvironmentEntityProvider
{
    private readonly IDictionary<Type, IEnvironmentEntity> _environmentEntityMap;

    public EnvironmentEntityProvider(IEnumerable<IEnvironmentEntity> environmentEntities)
    {
        _environmentEntityMap = environmentEntities
            .ToDictionary(entity => entity.GetType(), entity => entity);
    }

    public TEnvironmentEntity Get<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        return (TEnvironmentEntity)_environmentEntityMap[typeof(TEnvironmentEntity)];
    }
}