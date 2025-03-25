using System;

namespace Environment.Setup;

public sealed record EnvironmentConfigurationState(Func<object> ConfigurationEnricher, bool IsObservable)
{
    public Func<object> ConfigurationEnricher { get; internal set; } = ConfigurationEnricher;
    
    public bool IsObservable { get; internal set; } = IsObservable;
}