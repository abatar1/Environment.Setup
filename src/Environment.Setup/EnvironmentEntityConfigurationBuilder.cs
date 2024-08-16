using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public sealed class EnvironmentEntityConfigurationBuilder(IServiceCollection serviceCollection)
{
    public TEnvironmentEntity AddEntity<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var environmentSetup = (TEnvironmentEntity?)Activator.CreateInstance<TEnvironmentEntity>() 
                               ?? throw new InvalidOperationException($"Could not create instance of {typeof(TEnvironmentEntity)} with empty ctor.");
        serviceCollection.AddSingleton(typeof(IEnvironmentEntity), environmentSetup);
        return environmentSetup;
    }
    
    public TEnvironmentEntity AddEntity<TEnvironmentEntity>(Func<TEnvironmentEntity> entityEnricher)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        var environmentSetup = entityEnricher.Invoke();
        serviceCollection.AddSingleton(typeof(IEnvironmentEntity), environmentSetup);
        return environmentSetup;
    }
}