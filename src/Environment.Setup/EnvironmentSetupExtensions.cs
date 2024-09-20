using System;
using System.Linq.Expressions;

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
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        return setup.SetupProperty(setupEnricher, environmentVariableName, x => x, isRequired);
    }
    
    public static TEnvironmentEntity SetupLongProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, long>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
        where TEnvironmentEntity : class, IEnvironmentEntity
    {
        return setup.SetupProperty(setupEnricher, environmentVariableName, long.Parse, isRequired);
    }
    
    public static TEnvironmentEntity SetupIntProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, int>> setupEnricher,
        string environmentVariableName,
        bool isRequired = true)
        where TEnvironmentEntity : class, IEnvironmentEntity
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
        where TEnvironmentEntity : class, IEnvironmentEntity
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

        return setup.SetupProperty(setupEnricher, convertedValue, environmentVariableName);
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            convertedValue = converter.Invoke(environmentVariable.Trim());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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