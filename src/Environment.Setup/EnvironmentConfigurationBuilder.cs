using System;
using System.Collections.Generic;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationBuilder(IServiceProvider serviceProvider, Dictionary<Type, EnvironmentConfigurationState> states)
{
    internal Dictionary<Type, EnvironmentConfigurationState> States { get; } = states;
    
    internal readonly IServiceProvider ServiceProvider = serviceProvider;

    /// <summary>
    /// Configures the specified environment entity by invoking the provided entity enricher.
    /// </summary>
    /// <typeparam name="TEnvironmentConfiguration">The type of the environment configuration to be set up.</typeparam>
    /// <param name="entityEnricher">An action that provides configuration details for the environment entity.</param>
    /// <returns>An instance of <see cref="EnvironmentConfigurationConcreteBuilder"/> representing the configuration context for the specified environment entity.</returns>
    public EnvironmentConfigurationConcreteBuilder Configure<TEnvironmentConfiguration>(
        Action<EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var entity = Activator.CreateInstance<TEnvironmentConfiguration>();
        
        var builder = new EnvironmentConfigurationBuilder<TEnvironmentConfiguration>(ServiceProvider, entity);
        entityEnricher.Invoke(builder);
        
        var enricher = builder.Build();
        
        TryAddEntity(enricher);
        
        return new EnvironmentConfigurationConcreteBuilder(typeof(TEnvironmentConfiguration), this);
    }

    /// <summary>
    /// Configures the specified environment configuration by invoking the provided entity enricher.
    /// </summary>
    /// <typeparam name="TEnvironmentConfiguration">The type of the environment configuration to be configured.</typeparam>
    /// <param name="entityEnricher">An action that configures the environment entity using the service provider and a configuration builder.</param>
    /// <returns>An instance of <see cref="EnvironmentConfigurationConcreteBuilder"/> representing the configuration context for the specified environment entity.</returns>
    public EnvironmentConfigurationConcreteBuilder Configure<TEnvironmentConfiguration>(
        Action<IServiceProvider, EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var entity = Activator.CreateInstance<TEnvironmentConfiguration>();
        
        var builder = new EnvironmentConfigurationBuilder<TEnvironmentConfiguration>(ServiceProvider, entity);
        entityEnricher.Invoke(ServiceProvider, builder);
        
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