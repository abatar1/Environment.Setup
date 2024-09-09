namespace Environment.Setup;

public interface IEnvironmentEntityProvider
{
    /// <summary>
    /// Provides environment variable entity by type. Entity means "container" class, containing environment variables, grouped by some logic, e.g. by service-consumer name.
    /// </summary>
    TEnvironmentEntity Get<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity;
}