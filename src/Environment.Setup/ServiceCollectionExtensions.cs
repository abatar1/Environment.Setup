using System;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setups environments, builder used to set up each variable separately. Use IEnvironmentEntityProvider to get variables.
    /// </summary>
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Func<EnvironmentEntityConfigurationBuilder, EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>(sp =>
        {
            var builder = new EnvironmentEntityConfigurationBuilder(sp);
            builderEnricher.Invoke(builder);
            builder = builderEnricher.Invoke(builder);
            return new EnvironmentEntityProvider(builder.Entities);
        });
        
        return serviceCollection;
    }
    
    /// <summary>
    /// Setups environments, builder used to set up each variable separately. Use IEnvironmentEntityProvider to get variables.
    /// Allows to use service provider during set up.
    /// </summary>
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Func<IServiceProvider, EnvironmentEntityConfigurationBuilder, EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>(sp =>
        {
            var builder = new EnvironmentEntityConfigurationBuilder(sp);
            builder = builderEnricher.Invoke(sp, builder);
            return new EnvironmentEntityProvider(builder.Entities);
        });
        
        return serviceCollection;
    }
}
