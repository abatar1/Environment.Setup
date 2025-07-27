using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up the environment observer by configuring environment-specific state using a provided builder enricher function.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the environment configurations will be added.</param>
    /// <param name="builderEnricher">A function that enriches the <see cref="EnvironmentConfigurationStatesBuilder"/> with custom configurations.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> containing the configured environment observers.</returns>
    public static IServiceCollection SetupEnvironmentObserver(this IServiceCollection services, Func<EnvironmentConfigurationStatesBuilder, EnvironmentConfigurationStatesBuilder> builderEnricher)
    {
        var builder = new EnvironmentConfigurationStatesBuilder(new Dictionary<Type, EnvironmentConfigurationState>());
        var concreteBuilder = builderEnricher.Invoke(builder);

        return RegisterConfigurationsFromStates(services, concreteBuilder.States);
    }

    /// <summary>
    /// Configures the environment observer by applying custom state configurations through a provided builder enricher function.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance where the environment configurations will be registered.</param>
    /// <param name="builderEnricher">A function that enriches the <see cref="EnvironmentConfigurationStatesBuilder"/> and returns a concrete builder with defined configurations.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> including the registered environment configurations.</returns>
    internal static IServiceCollection SetupEnvironmentObserver(this IServiceCollection services, Func<EnvironmentConfigurationStatesBuilder, EnvironmentConfigurationConcreteBuilder> builderEnricher)
    {
        var builder = new EnvironmentConfigurationStatesBuilder(new Dictionary<Type, EnvironmentConfigurationState>());
        var concreteBuilder = builderEnricher.Invoke(builder);
        
        return RegisterConfigurationsFromStates(services, concreteBuilder.States);
    }

    /// <summary>
    /// Configures and registers environment-specific settings and observers into the service collection using a custom enricher function
    /// that allows interaction with both the configuration builder and the service provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where the environment configurations will be registered.</param>
    /// <param name="builderEnricher">A function that customizes the <see cref="EnvironmentConfigurationStatesBuilder"/> by leveraging an
    /// instance of <see cref="IServiceProvider"/> for further service dependency access.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> containing the registered environment-specific configurations and observers.</returns>
    public static IServiceCollection SetupEnvironmentObserver(this IServiceCollection services, Func<EnvironmentConfigurationStatesBuilder, IServiceProvider, EnvironmentConfigurationStatesBuilder> builderEnricher)
    {
        var builder = new EnvironmentConfigurationStatesBuilder(new Dictionary<Type, EnvironmentConfigurationState>());
        var serviceProvider = services.BuildServiceProvider();
        var concreteBuilder = builderEnricher.Invoke(builder, serviceProvider);
        
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
