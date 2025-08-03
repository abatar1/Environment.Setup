using System.IO;

namespace Environment.Setup;

internal sealed class EnvironmentFileReader(string name, string path) : IEnvironmentReader
{
    /// <summary>
    /// Retrieves the value of an environment configuration from the specified source.
    /// </summary>
    /// <returns>The value of the environment configuration.</returns>
    /// <exception cref="EnvironmentVariableNullException">Thrown when the environment configuration value is null, empty, or whitespace.</exception>
    public string GetValue()
    {
        var filePath = Path.Combine(path, name);
        var environmentVariable = File.ReadAllText(filePath).Trim();
        if (string.IsNullOrWhiteSpace(environmentVariable))
            throw new EnvironmentVariableNullException($"Failed to load {name} file on path {path}, ensure it exists");
        return environmentVariable;
    }
}