using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Action<EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>();
        var builder = new EnvironmentEntityConfigurationBuilder(serviceCollection);
        builderEnricher.Invoke(builder);
        return serviceCollection;
    }
}