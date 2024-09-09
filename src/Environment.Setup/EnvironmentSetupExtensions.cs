using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Environment.Setup;

/// <summary>
/// Extensions class to set up environment entity in builder.
/// </summary>
public static class EnvironmentSetupExtensions
{
    public static TEnvironmentEntity SetupStringProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, string>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        return setup.SetupProperty(setupEnricher, environmentVariableName, x => x, isRequired);
    }
    
    public static TEnvironmentEntity SetupLongProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, long>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        return setup.SetupProperty(setupEnricher, environmentVariableName, long.Parse, isRequired);
    }
    
    public static TEnvironmentEntity SetupIntProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, int>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        return setup.SetupProperty(setupEnricher, environmentVariableName, int.Parse, isRequired);
    }

    /// <summary>
    /// Used if environment variable contains custom class which requires conversion. 
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static TEnvironmentEntity SetupProperty<TValue, TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, TValue>> setupEnricher,
        string environmentVariableName,
        Func<string, TValue> converter,
        bool isRequired = true)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        TValue convertedValue;
        try
        {
            convertedValue = GetEnvironmentVariable(environmentVariableName, converter);
        }
        catch (EnvironmentVariableNullException)
        {
            if (isRequired)
                throw;
            return setup;
        }

        var memberExpression = setupEnricher.Body as MemberExpression;
        var property = memberExpression?.Member as PropertyInfo;
        if (property == null)
            throw new EnvironmentSetupException($"Could not get the property of environment setup for environment variable {environmentVariableName}, ensure property selected correctly");

        try
        {
            property.SetValue(setup, convertedValue);
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException(
                $"Failed to set property {property.Name} with value {convertedValue} for variable {environmentVariableName}",
                e);
        }

        return setup;
    }
    
    private static TValue GetEnvironmentVariable<TValue>(
        string environmentVariableName,
        Func<string, TValue> converter)
    {
        var environmentVariable = System.Environment.GetEnvironmentVariable(environmentVariableName)?.Trim();
        if (string.IsNullOrWhiteSpace(environmentVariable))
            throw new EnvironmentVariableNullException($"Failed to load {environmentVariableName} environment variable, ensure it has been set up");

        TValue convertedValue;
        try
        {
            convertedValue = converter.Invoke(environmentVariable.Trim());
        }
        catch (Exception e)
        {
            throw new EnvironmentSetupException(
                $"Failed to convert value {environmentVariable} for {environmentVariableName} environment variable, ensure it has been set up correctly",
                e);
        }

        return convertedValue;
    }
}