using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Environment.Setup;

public static class EnvironmentSetupExtensions
{
    public static void SetupStringProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, string>> setupEnricher,
        string environmentVariableName)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        setup.SetupProperty(setupEnricher, environmentVariableName, x => x);
    }

    public static void SetupLongProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, long>> setupEnricher,
        string environmentVariableName)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        setup.SetupProperty(setupEnricher, environmentVariableName, long.Parse);
    }
    
    public static void SetupIntProperty<TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, int>> setupEnricher,
        string environmentVariableName)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        setup.SetupProperty(setupEnricher, environmentVariableName, int.Parse);
    }
    
    public static int GetIntVariable<TEnvironmentEntity>(
        this TEnvironmentEntity _,
        string environmentVariableName)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        return GetEnvironmentVariable(environmentVariableName, int.Parse);
    }

    public static void SetupProperty<TValue, TEnvironmentEntity>(
        this TEnvironmentEntity setup,
        Expression<Func<TEnvironmentEntity, TValue>> setupEnricher,
        string environmentVariableName,
        Func<string, TValue> converter)
        where TEnvironmentEntity : IEnvironmentEntity
    {
        var convertedValue = GetEnvironmentVariable(environmentVariableName, converter);

        var memberExpression = setupEnricher.Body as MemberExpression;
        var property = memberExpression?.Member as PropertyInfo;
        if (property == null)
            throw new EnvironmentSetupException(
                $"Could not get the property of environment setup for environment variable {environmentVariableName}, ensure property selected correctly");

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
    }
    
    private static TValue GetEnvironmentVariable<TValue>(
        string environmentVariableName,
        Func<string, TValue> converter)
    {
        var environmentVariable = System.Environment.GetEnvironmentVariable(environmentVariableName)?.Trim();
        if (string.IsNullOrWhiteSpace(environmentVariable))
            throw new EnvironmentSetupException(
                $"Failed to load {environmentVariableName} environment variable, ensure it has been set up");

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