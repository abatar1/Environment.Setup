namespace Environment.Setup;

internal sealed class EnvironmentVariableReader(string name) : IEnvironmentReader
{
    /// <summary>
    /// Retrieves the value of the specified environment variable.
    /// </summary>
    /// <returns>
    /// A string containing the value of the environment variable.
    /// </returns>
    /// <exception cref="EnvironmentVariableNullException">
    /// Thrown when the environment variable is null, empty, or consists only of whitespace.
    /// </exception>
    public string GetValue()
    {
        var environmentVariable = System.Environment.GetEnvironmentVariable(name)?.Trim();
        if (string.IsNullOrWhiteSpace(environmentVariable))
            throw new EnvironmentVariableNullException($"Failed to load {name} environment variable, ensure it has been set up");
        return environmentVariable;
    }
}