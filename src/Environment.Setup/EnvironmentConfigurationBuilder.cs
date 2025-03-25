using System;
using System.Collections.Generic;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationBuilder(IServiceProvider serviceProvider, Dictionary<Type, EnvironmentConfigurationState> states)
{
    internal Dictionary<Type, EnvironmentConfigurationState> States { get; } = states;
    
    internal readonly IServiceProvider ServiceProvider = serviceProvider;

    public EnvironmentConfigurationConcreteBuilder Configure<TEnvironmentConfiguration>(Action<EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var entity = Activator.CreateInstance<TEnvironmentConfiguration>();
        
        var builder = new EnvironmentConfigurationBuilder<TEnvironmentConfiguration>(ServiceProvider, entity);
        entityEnricher.Invoke(builder);
        
        var enricher = builder.Build();
        
        TryAddEntity(enricher);
        
        return new EnvironmentConfigurationConcreteBuilder(typeof(TEnvironmentConfiguration), this);
    }

    private void TryAddEntity<TEnvironmentConfiguration>(Func<TEnvironmentConfiguration> configurationEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        try
        {
            States.Add(typeof(TEnvironmentConfiguration), new EnvironmentConfigurationState(configurationEnricher, false));
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException($"Entity of type {typeof(TEnvironmentConfiguration).FullName} already registered", e);
        }
    }
}