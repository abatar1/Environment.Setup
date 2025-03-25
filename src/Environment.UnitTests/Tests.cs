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
        var envKey = "key";
        var envValue1 = "value1";
        var envValue2 = "value2";
        
        var stack = new Stack<string>();
        stack.Push(envValue2);
        stack.Push(envValue1);
        
        var services = new ServiceCollection();
        var providerMock = new Mock<IEnvironmentVariableProvider>();
        providerMock
            .Setup(x => x.GetEnvironmentVariable(It.Is<string>(y => y == envKey), It.IsAny<Func<string, string>>()))
            .Returns(() => stack.Pop());
        
        // Act
        services.SetupEnvironmentObserver(providerMock.Object, x => 
            x.Configure<TestConfiguration>(c => 
                c.WithStringVariable(p => p.TestStringProperty, envKey)));
        
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
        var envKey = "key";
        var envValue1 = "value1";
        var envValue2 = "value2";
        
        var stack = new Stack<string>();
        stack.Push(envValue2);
        stack.Push(envValue1);
        
        var services = new ServiceCollection();
        var providerMock = new Mock<IEnvironmentVariableProvider>();
        providerMock
            .Setup(x => x.GetEnvironmentVariable(It.Is<string>(y => y == envKey), It.IsAny<Func<string, string>>()))
            .Returns(() => stack.Pop());
        
        // Act
        services.SetupEnvironmentObserver(providerMock.Object,x => 
            x.Configure<TestConfiguration>(c => c.WithStringVariable(p => p.TestStringProperty, envKey)).AsObservable());
        
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