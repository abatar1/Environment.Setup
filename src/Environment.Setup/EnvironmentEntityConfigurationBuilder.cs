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
        TryAddEntity(typeof(TEnvironmentEntity), entity);
        return this;
    }
    
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>(Action<TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var entity = Activator.CreateInstance<TEnvironmentEntity>();
        entityEnricher.Invoke(entity);
        TryAddEntity(typeof(TEnvironmentEntity), entity);
        return this;
    }

    private void TryAddEntity(Type type, IEnvironmentEntity entity)
    {
        try
        {
            Entities.Add(type, entity);
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException($"Entity of type {type.FullName} already registered", e);
        }
    }

    internal IDictionary<Type, IEnvironmentEntity> Entities { get; } = new Dictionary<Type, IEnvironmentEntity>();
}