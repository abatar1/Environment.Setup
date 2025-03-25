using System;

namespace Environment.Setup;

internal sealed class EnvironmentVariableProvider : IEnvironmentVariableProvider
{
    public TValue GetEnvironmentVariable<TValue>(string environmentVariableName, Func<string, TValue> converter)
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