using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setups environments, builder used to setup each variable separately. Use IEnvironmentEntityProvider to get variables.
    /// </summary>
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Action<EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>();
        var builder = new EnvironmentEntityConfigurationBuilder(serviceCollection);
        builderEnricher.Invoke(builder);
        return serviceCollection;
    }
}