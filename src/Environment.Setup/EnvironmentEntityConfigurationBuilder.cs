using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public sealed class EnvironmentEntityConfigurationBuilder(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
{
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>(Action<IServiceProvider, TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var entity = Activator.CreateInstance<TEnvironmentEntity>();
        entityEnricher.Invoke(serviceProvider, entity);
        serviceCollection.AddSingleton(typeof(IEnvironmentEntity), entity);
        return this;
    }
    
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>(Action<TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var entity = Activator.CreateInstance<TEnvironmentEntity>();
        entityEnricher.Invoke(entity);
        serviceCollection.AddSingleton(typeof(IEnvironmentEntity), entity);
        return this;
    }
}