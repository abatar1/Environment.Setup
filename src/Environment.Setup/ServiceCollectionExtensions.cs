using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setups environments, builder used to set up each variable separately. Use IEnvironmentEntityProvider to get variables.
    /// </summary>
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Action<EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>(sp =>
        {
            var builder = new EnvironmentEntityConfigurationBuilder(serviceCollection, sp);
            builderEnricher.Invoke(builder);
            var entities = sp.GetRequiredService<IEnumerable<IEnvironmentEntity>>();
            return new EnvironmentEntityProvider(entities);
        });
        
        return serviceCollection;
    }
    
    /// <summary>
    /// Setups environments, builder used to set up each variable separately. Use IEnvironmentEntityProvider to get variables.
    /// Allows to use service provider during set up.
    /// </summary>
    public static IServiceCollection SetupEnvironment(this IServiceCollection serviceCollection, Action<IServiceProvider, EnvironmentEntityConfigurationBuilder> builderEnricher)
    {
        serviceCollection.AddSingleton<IEnvironmentEntityProvider, EnvironmentEntityProvider>(sp =>
        {
            var builder = new EnvironmentEntityConfigurationBuilder(serviceCollection, sp);
            builderEnricher.Invoke(sp, builder);
            var entities = sp.GetRequiredService<IEnumerable<IEnvironmentEntity>>();
            return new EnvironmentEntityProvider(entities);
        });
        
        return serviceCollection;
    }
}