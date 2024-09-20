using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Environment.Setup;

internal static class PropertySetterHelper
{
    public static TEnvironmentEntity SetupProperty<TEnvironmentEntity, TValue>(this TEnvironmentEntity environmentEntity, 
        Expression<Func<TEnvironmentEntity, TValue>> setupEnricher,
        TValue value, 
        string environmentVariableName)
        where TEnvironmentEntity : class, IEnvironmentEntity
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