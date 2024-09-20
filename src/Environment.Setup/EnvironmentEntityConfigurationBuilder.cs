using System;
using System.Collections.Generic;

namespace Environment.Setup;

public sealed class EnvironmentEntityConfigurationBuilder(IServiceProvider serviceProvider)
{
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>(Action<IServiceProvider, TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var entity = Activator.CreateInstance<TEnvironmentEntity>();
        entityEnricher.Invoke(serviceProvider, entity);
        Entities.Add(typeof(TEnvironmentEntity), entity);
        return this;
    }
    
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>(Action<TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var entity = Activator.CreateInstance<TEnvironmentEntity>();
        entityEnricher.Invoke(entity);
        Entities.Add(typeof(TEnvironmentEntity), entity);
        return this;
    }

    internal IDictionary<Type, IEnvironmentEntity> Entities { get; } = new Dictionary<Type, IEnvironmentEntity>();
}