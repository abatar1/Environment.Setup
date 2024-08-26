using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public sealed class EnvironmentEntityConfigurationBuilder(IServiceCollection serviceCollection)
{
    public EnvironmentEntityConfigurationBuilder AddEntity<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var environmentSetup = (TEnvironmentEntity?)Activator.CreateInstance<TEnvironmentEntity>() 
                               ?? throw new InvalidOperationException($"Could not create instance of {typeof(TEnvironmentEntity)} with empty ctor.");
        serviceCollection.AddSingleton(typeof(IEnvironmentEntity), environmentSetup);
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