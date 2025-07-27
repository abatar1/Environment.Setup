using System;
using System.Collections.Generic;

namespace Environment.Setup;

/// <summary>
/// Represents a concrete builder used for configuring environment-specific configurations. This class is responsible for managing
/// the state of environment configurations and facilitating the application of detailed configuration enrichments for
/// specified entities.
/// </summary>
public sealed class EnvironmentConfigurationConcreteBuilder(Type type, EnvironmentConfigurationStatesBuilder configurationBuilder)
{
    internal Dictionary<Type, EnvironmentConfigurationState> States { get; } = configurationBuilder.States;
    
    private readonly EnvironmentConfigurationStatesBuilder _configurationBuilder = configurationBuilder;

    /// <summary>
    /// Configures the environment with specific settings for the type of environment configuration provided.
    /// </summary>
    /// <typeparam name="TEnvironmentConfiguration">The type of the environment configuration.</typeparam>
    /// <param name="entityEnricher">Action that enriches the environment configuration builder with the required settings.</param>
    /// <returns>An instance of <see cref="EnvironmentConfigurationStatesBuilder"/> for chaining further configurations.</returns>
    public EnvironmentConfigurationStatesBuilder Configure<TEnvironmentConfiguration>(
        Action<EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var concreteBuilder = _configurationBuilder.Configure(entityEnricher);
        return concreteBuilder._configurationBuilder;
    }

    /// <summary>
    /// Marks the current environment configuration as observable, enabling dynamic updates to the configuration's state when changes occur.
    /// </summary>
    /// <returns>An instance of <see cref="EnvironmentConfigurationStatesBuilder"/> for further configuration chaining.</returns>
    public EnvironmentConfigurationStatesBuilder AsObservable()
    {
        var states = _configurationBuilder.States;
        states[type] = states[type] with { IsObservable = true };
        return new EnvironmentConfigurationStatesBuilder(states);
    }
}