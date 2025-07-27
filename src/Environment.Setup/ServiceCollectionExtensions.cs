using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setups environment observer, builder used to set up each variable separately. 
    /// </summary>
    public static IServiceCollection SetupEnvironmentObserver(this IServiceCollection services, Func<EnvironmentConfigurationBuilder, EnvironmentConfigurationBuilder> builderEnricher)
    {
        var builder = new EnvironmentConfigurationBuilder(new Dictionary<Type, EnvironmentConfigurationState>());
        var concreteBuilder = builderEnricher.Invoke(builder);

        return RegisterConfigurationsFromStates(services, concreteBuilder.States);
    }
    
    public static IServiceCollection SetupEnvironmentObserver(this IServiceCollection services, Func<EnvironmentConfigurationBuilder, EnvironmentConfigurationConcreteBuilder> builderEnricher)
    {
        var builder = new EnvironmentConfigurationBuilder(new Dictionary<Type, EnvironmentConfigurationState>());
        var concreteBuilder = builderEnricher.Invoke(builder);
        
        return RegisterConfigurationsFromStates(services, concreteBuilder.States);
    }

    private static IServiceCollection RegisterConfigurationsFromStates(IServiceCollection services, Dictionary<Type, EnvironmentConfigurationState> states)
    {
        foreach (var state in states)
        {
            if (state.Value.IsObservable)
                services.AddTransient(state.Key, _ => state.Value.ConfigurationEnricher.Invoke());
            else
                services.AddSingleton(state.Key, state.Value.ConfigurationEnricher.Invoke());
        }

        return services;
    }
}
