using Environment.Setup;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Environment.UnitTests;

public sealed class Tests
{
    [Fact]
    public void Configure_NotObservable_ValueShouldNotChange()
    {
        // Assign
        var envValue1 = "value1";
        var envValue2 = "value2";
        
        var stack = new Stack<string>();
        stack.Push(envValue2);
        stack.Push(envValue1);
        
        var services = new ServiceCollection();
        var environmentReaderMock = new Mock<IEnvironmentReader>();
        environmentReaderMock
            .Setup(x => x.GetValue())
            .Returns(() => stack.Pop());
        
        // Act
        services.SetupEnvironmentObserver(x => 
            x.Configure<TestConfiguration>(c => 
                c.WithStringVariable(p => p.TestStringProperty, environmentReaderMock.Object)));
        
        // Assert
        using (var scope = services.BuildServiceProvider().CreateAsyncScope())
        {
            var testEntity = scope.ServiceProvider.GetRequiredService<TestConfiguration>();
            Assert.Equal(envValue1, testEntity.TestStringProperty);
        }
        
        using (var scope = services.BuildServiceProvider().CreateAsyncScope())
        {
            var testEntity = scope.ServiceProvider.GetRequiredService<TestConfiguration>();
            Assert.Equal(envValue1, testEntity.TestStringProperty);
        }
    }
    
    [Fact]
    public void Configure_Observable_ValueShouldNotChange()
    {
        // Assign
        var envValue1 = "value1";
        var envValue2 = "value2";
        
        var stack = new Stack<string>();
        stack.Push(envValue2);
        stack.Push(envValue1);
        
        var services = new ServiceCollection();
        var environmentReaderMock = new Mock<IEnvironmentReader>();
        environmentReaderMock
            .Setup(x => x.GetValue())
            .Returns(() => stack.Pop());
        
        // Act
        services.SetupEnvironmentObserver(x => 
            x.Configure<TestConfiguration>(c => c.WithStringVariable(p => p.TestStringProperty, environmentReaderMock.Object)).AsObservable());
        
        // Assert
        using (var scope = services.BuildServiceProvider().CreateAsyncScope())
        {
            var testEntity = scope.ServiceProvider.GetRequiredService<TestConfiguration>();
            Assert.Equal(envValue1, testEntity.TestStringProperty);
        }
        
        using (var scope = services.BuildServiceProvider().CreateAsyncScope())
        {
            var testEntity = scope.ServiceProvider.GetRequiredService<TestConfiguration>();
            Assert.Equal(envValue2, testEntity.TestStringProperty);
        }
    }
}