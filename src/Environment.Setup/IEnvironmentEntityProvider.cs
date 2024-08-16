namespace Environment.Setup;

public interface IEnvironmentEntityProvider
{
    TEnvironmentEntity Get<TEnvironmentEntity>()
        where TEnvironmentEntity : class, IEnvironmentEntity;
}