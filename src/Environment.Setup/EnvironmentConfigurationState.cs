using System;

namespace Environment.Setup;

/// <summary>
/// Represents the state of an environment configuration, encapsulating a configuration enricher function
/// and a flag indicating whether the configuration is observable.
/// </summary>
public sealed record EnvironmentConfigurationState(Func<object> ConfigurationEnricher, bool IsObservable)
{
    public Func<object> ConfigurationEnricher { get; internal set; } = ConfigurationEnricher;
    
    public bool IsObservable { get; internal set; } = IsObservable;
}