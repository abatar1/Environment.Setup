using System;
using System.Linq.Expressions;

namespace Environment.Setup;

public interface IEnvironmentEntityProvider
{
    /// <summary>
    /// Provides environment variable entity by type. Entity means "container" class, containing environment variables, grouped by some logic, e.g. by service-consumer name.
    /// If entity may be updated during runtime, Get should be called every time as you want to obtain it's value.
    /// </summary>
    TEnvironmentEntity Get<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity;

    /// <summary>
    /// Sets new value for entity property. Keep in mind that updated value located only in IEnvironmentEntityProvider because TEnvironmentEntity does not track updates.
    /// </summary>
    TEnvironmentEntity Set<TEnvironmentEntity, TValue>(Expression<Func<TEnvironmentEntity, TValue>> entityEnricher,
        TValue value)
        where TEnvironmentEntity : class, IEnvironmentEntity;
}