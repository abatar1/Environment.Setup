using System;

namespace Environment.Setup;

public interface IEnvironmentVariableProvider
{
    TValue GetEnvironmentVariable<TValue>(string environmentVariableName, Func<string, TValue> converter);
}