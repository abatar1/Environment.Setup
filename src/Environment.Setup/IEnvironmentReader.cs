namespace Environment.Setup;

/// <summary>
/// Represents a contract for reading environment values.
/// Provides functionalities to retrieve values from various environment sources
/// like environment variables or configuration files.
/// </summary>
public interface IEnvironmentReader
{
    string GetValue();
}