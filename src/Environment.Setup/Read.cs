namespace Environment.Setup;

/// <summary>
/// Provides utility methods for accessing environment settings from different sources such as files or environment variables.
/// </summary>
public static class Read
{
    /// <summary>
    /// Reads environment settings from a specified file and returns an environment reader instance.
    /// </summary>
    /// <param name="name">The name of the environment configuration file to be read.</param>
    /// <param name="filePath">The directory path where the configuration file is located.</param>
    /// <returns>An instance of <see cref="IEnvironmentReader"/> to read the environment configuration from the specified file.</returns>
    public static IEnvironmentReader FromFile(string name, string filePath)
    {
        return new EnvironmentFileReader(name, filePath);
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