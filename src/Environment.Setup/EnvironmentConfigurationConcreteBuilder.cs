using System;
using System.Collections.Generic;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationConcreteBuilder(Type type, EnvironmentConfigurationBuilder configurationBuilder)
{
    internal Dictionary<Type, EnvironmentConfigurationState> States { get; } = configurationBuilder.States;
    
    private readonly EnvironmentConfigurationBuilder _configurationBuilder = configurationBuilder;

    /// <summary>
    /// Configures the environment with specific settings for the type of environment configuration provided.
    /// </summary>
    /// <typeparam name="TEnvironmentConfiguration">The type of the environment configuration.</typeparam>
    /// <param name="entityEnricher">Action that enriches the environment configuration builder with the required settings.</param>
    /// <returns>An instance of <see cref="EnvironmentConfigurationBuilder"/> for chaining further configurations.</returns>
    public EnvironmentConfigurationBuilder Configure<TEnvironmentConfiguration>(
        Action<EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var concreteBuilder = _configurationBuilder.Configure(entityEnricher);
        return concreteBuilder._configurationBuilder;
    }

    /// <summary>
    /// Marks the current environment configuration as observable, enabling dynamic updates to the configuration's state when changes occur.
    /// </summary>
    /// <returns>An instance of <see cref="EnvironmentConfigurationBuilder"/> for further configuration chaining.</returns>
    public EnvironmentConfigurationBuilder AsObservable()
    {
        var states = _configurationBuilder.States;
        states[type] = states[type] with { IsObservable = true };
        return new EnvironmentConfigurationBuilder(states);
    }
}