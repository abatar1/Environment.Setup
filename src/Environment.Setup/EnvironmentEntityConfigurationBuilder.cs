using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Environment.Setup;

public sealed class EnvironmentConfigurationBuilder<TEnvironmentConfiguration>(TEnvironmentConfiguration configuration)
    where TEnvironmentConfiguration : class, IEnvironmentConfiguration
{
    private readonly List<Func<TEnvironmentConfiguration, TEnvironmentConfiguration>> _enrichers = new();

    /// <summary>
    /// Enriches the configuration by setting up a string variable from the environment.
    /// </summary>
    /// <param name="setupEnricher">An expression that sets the value of the configuration variable as a string.</param>
    /// <param name="environmentReader">The environment reader used to fetch the variable value.</param>
    /// <param name="isRequired">A boolean indicating if the variable is required. If true, an exception will be thrown if the variable is not set.</param>
    /// <returns>The same configuration builder instance, allowing for method chaining.</returns>
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithStringVariable(
        Expression<Func<TEnvironmentConfiguration, string>> setupEnricher,
        IEnvironmentReader environmentReader,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentReader, x => x, isRequired);
        return this;
    }

    /// <summary>
    /// Enriches the configuration by setting up a long variable from the environment.
    /// </summary>
    /// <param name="setupEnricher">An expression that sets the value of the configuration variable as a long.</param>
    /// <param name="environmentReader">The environment reader used to fetch the variable value.</param>
    /// <param name="isRequired">A boolean indicating if the variable is required. If true, an exception will be thrown if the variable is not set.</param>
    /// <returns>The same configuration builder instance, allowing for method chaining.</returns>
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithLongVariable(
        Expression<Func<TEnvironmentConfiguration, long>> setupEnricher,
        IEnvironmentReader environmentReader,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentReader, long.Parse, isRequired);
        return this;
    }

    /// <summary>
    /// Enriches the configuration by setting up an integer variable from the environment.
    /// </summary>
    /// <param name="setupEnricher">An expression that sets the value of the configuration variable as an integer.</param>
    /// <param name="environmentReader">The environment reader used to fetch the variable value.</param>
    /// <param name="isRequired">A boolean indicating if the variable is required. If true, an exception will be thrown if the variable is not set.</param>
    /// <returns>The same configuration builder instance, allowing for method chaining.</returns>
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithIntVariable(
        Expression<Func<TEnvironmentConfiguration, int>> setupEnricher,
        IEnvironmentReader environmentReader,
        bool isRequired = true)
    {
        WithVariable(setupEnricher, environmentReader, int.Parse, isRequired);
        return this;
    }

    /// <summary>
    /// Enriches the configuration by setting up a variable from the environment.
    /// </summary>
    /// <typeparam name="TValue">The type of the variable being configured.</typeparam>
    /// <param name="setupEnricher">An expression that sets the value of the configuration variable.</param>
    /// <param name="environmentReader">The environment reader used to fetch the variable value.</param>
    /// <param name="converter">A function to convert the environment variable's string value to the desired type.</param>
    /// <param name="isRequired">A boolean indicating if the variable is required. If true, an exception will be thrown if the variable is not set.</param>
    /// <returns>The same configuration builder instance, allowing for method chaining.</returns>
    public EnvironmentConfigurationBuilder<TEnvironmentConfiguration> WithVariable<TValue>(
        Expression<Func<TEnvironmentConfiguration, TValue>> setupEnricher,
        IEnvironmentReader environmentReader,
        Func<string, TValue> converter,
        bool isRequired = true)
    {
        Func<TEnvironmentConfiguration, TEnvironmentConfiguration> enricher = c => SetupBaseVariable(c, setupEnricher, environmentReader, converter, isRequired);
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
        IEnvironmentReader environmentReader,
        Func<string, TValue> converter,
        bool isRequired = true)
    {
        string value;
        try
        {
            value = environmentReader.GetValue();
        }
        catch (EnvironmentVariableNullException)
        {
            if (isRequired)
                throw;
            return config;
        }
        
        TValue convertedValue;
        try
        {
            convertedValue = converter.Invoke(value);
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException($"Failed to convert value {value} for  environment variable, ensure it has been set up correctly", e);
        }

        try
        {
            return SetProperty(config, setupEnricher, convertedValue);
        }
        catch (EnvironmentSetupException e)
        {
            throw new EnvironmentSetupException($"{value}: {e.Message}");
        }
        
    }
    
    private static TEnvironmentConfiguration SetProperty<TValue>(TEnvironmentConfiguration environmentEntity, 
        Expression<Func<TEnvironmentConfiguration, TValue>> setupEnricher,
        TValue value)
    {
        var memberExpression = setupEnricher.Body as MemberExpression;
        var property = memberExpression?.Member as PropertyInfo;
        if (property == null)
            throw new EnvironmentSetupException($"Could not get the property of environment setup for environment variable, ensure property selected correctly");

        try
        {
            property.SetValue(environmentEntity, value);
            return environmentEntity;
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException($"Failed to set property {property.Name} with value {value}", e);
        }
    }
}