namespace Environment.Setup;

/// <summary>
/// Provides utility methods for accessing environment settings from different sources such as files or environment variables.
/// </summary>
public static class Read
{
    /// <summary>
    /// Reads environment settings from a specified file and returns an environment reader instance.
    /// </summary>
    /// <param name="filePath">The path to the file containing environment configuration values.</param>
    /// <returns>An instance of <see cref="IEnvironmentReader"/> to read the environment configuration from the specified file.</returns>
    public static IEnvironmentReader FromFile(string filePath)
    {
        return new EnvironmentFileReader(filePath);
    }

    /// <summary>
    /// Reads environment settings from a specified environment variable and returns an environment reader instance.
    /// </summary>
    /// <param name="environmentVariableName">The name of the environment variable containing configuration values.</param>
    /// <returns>An instance of <see cref="IEnvironmentReader"/> to read the environment configuration from the specified environment variable.</returns>
    public static IEnvironmentReader FromVariable(string environmentVariableName)
    {
        return new EnvironmentVariableReader(environmentVariableName);
    }
}