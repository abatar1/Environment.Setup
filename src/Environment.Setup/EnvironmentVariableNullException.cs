using System;

namespace Environment.Setup;

internal sealed class EnvironmentVariableNullException : Exception
{
    public EnvironmentVariableNullException(string? message) : base(message)
    {
    }

    public EnvironmentVariableNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}