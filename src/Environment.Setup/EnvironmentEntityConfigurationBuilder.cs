using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationBuilder<TEnvironmentConfiguration>(IServiceProvider serviceProvider, TEnvironmentConfiguration configuration)
    where TEnvironmentConfiguration : class, IEnvironmentConfiguration
{
    private readonly List<Func<TEnvironmentConfiguration, TEnvironmentConfiguration>> _enrichers = new();
    private readonly IEnvironmentVariableProvider _provider = serviceProvider.GetRequiredService<IEnvironmentVariableProvider>();

    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithStringVariable(
        Expression<Func<TEnvironmentConfiguration, string>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentVariableName, x => x, isRequired);
        return this;
    }
    
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithLongVariable(
        Expression<Func<TEnvironmentConfiguration, long>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentVariableName, long.Parse, isRequired);
        return this;
    }
    
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithIntVariable(
        Expression<Func<TEnvironmentConfiguration, int>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentVariableName, int.Parse, isRequired);
        return this;
    }

    /// <summary>
    /// Used if environment variable contains custom class which requires conversion. 
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithVariable<TValue>(
        Expression<Func<TEnvironmentConfiguration, TValue>> setupEnricher,
        string environmentVariableName,
        Func<string, TValue> converter,
        bool isRequired = true)
    {
        Func<TEnvironmentConfiguration, TEnvironmentConfiguration> enricher = c => SetupBaseVariable(c, setupEnricher, environmentVariableName, converter, isRequired);
        _enrichers.Add(enricher);
        return this;
    }

    internal Func<TEnvironmentConfiguration> Build()
    {
        return () =>
        {
            var result = configuration;
            foreach (var enricher in _enrichers)
                result = enricher.Invoke(result);
            return result;
        };
    }
    
    private TEnvironmentConfiguration SetupBaseVariable<TValue>(
        TEnvironmentConfiguration config,
        Expression<Func<TEnvironmentConfiguration, TValue>> setupEnricher,
        string environmentVariableName,
        Func<string, TValue> converter,
        bool isRequired = true)
    {
        TValue convertedValue;
        try
        {
            convertedValue = _provider.GetEnvironmentVariable(environmentVariableName, converter);
        }
        catch (EnvironmentVariableNullException)
        {
            if (isRequired)
                throw;
            return config;
        }

        return SetProperty(config, setupEnricher, convertedValue, environmentVariableName);
    }
    
    private static TEnvironmentConfiguration SetProperty<TValue>(TEnvironmentConfiguration environmentEntity, 
        Expression<Func<TEnvironmentConfiguration, TValue>> setupEnricher,
        TValue value, 
        string environmentVariableName)
    {
        var memberExpression = setupEnricher.Body as MemberExpression;
        var property = memberExpression?.Member as PropertyInfo;
        if (property == null)
            throw new EnvironmentSetupException($"Could not get the property of environment setup for environment variable {environmentVariableName}, ensure property selected correctly");

        try
        {
            property.SetValue(environmentEntity, value);
            return environmentEntity;
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException(
                $"Failed to set property {property.Name} with value {value} for variable {environmentVariableName}",
                e);
        }
    }
}