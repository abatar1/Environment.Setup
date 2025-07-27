using System.IO;

namespace Environment.Setup;

internal sealed class EnvironmentFileReader(string name) : IEnvironmentReader
{
    /// <summary>
    /// Retrieves the value of an environment configuration from the specified source.
    /// </summary>
    /// <returns>The value of the environment configuration.</returns>
    /// <exception cref="EnvironmentVariableNullException">Thrown when the environment configuration value is null, empty, or whitespace.</exception>
    public string GetValue()
    {
        var environmentVariable = File.ReadAllText(name).Trim();
        if (string.IsNullOrWhiteSpace(environmentVariable))
            throw new EnvironmentVariableNullException($"Failed to load {name} file, ensure it exists");
        return environmentVariable;
    }
}