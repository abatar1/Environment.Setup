using System;
using System.Collections.Generic;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationConcreteBuilder(Type type, EnvironmentConfigurationBuilder configurationBuilder)
{
    internal Dictionary<Type, EnvironmentConfigurationState> States { get; } = configurationBuilder.States;
    
    private readonly EnvironmentConfigurationBuilder _configurationBuilder = configurationBuilder;
    
    public EnvironmentConfigurationBuilder Configure<TEnvironmentConfiguration>(Action<EnvironmentConfigurationBuilder<TEnvironmentConfiguration>> entityEnricher)
        where TEnvironmentConfiguration : class, IEnvironmentConfiguration
    {
        var concreteBuilder = _configurationBuilder.Configure(entityEnricher);
        return concreteBuilder._configurationBuilder;
    }
    
    public EnvironmentConfigurationBuilder AsObservable()
    {
        var states = _configurationBuilder.States;
        states[type] = states[type] with { IsObservable = true };
        return new EnvironmentConfigurationBuilder(_configurationBuilder.ServiceProvider, states);
    }
}