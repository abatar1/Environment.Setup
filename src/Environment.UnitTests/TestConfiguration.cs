using Environment.Setup;

namespace Environment.UnitTests;

internal sealed class TestConfiguration : IEnvironmentConfiguration
{
    public required string TestStringProperty { get; set; }
    
    public required int TestIntProperty { get; set; }
    
    public required long TestLongProperty { get; set; }
    
    public required TimeSpan TestTimeSpanProperty { get; set; }
}