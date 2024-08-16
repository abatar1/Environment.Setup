using System;

namespace Environment.Setup;

internal sealed class EnvironmentSetupException : Exception
{
    public EnvironmentSetupException(string? message) : base(message)
    {
    }

    public EnvironmentSetupException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}