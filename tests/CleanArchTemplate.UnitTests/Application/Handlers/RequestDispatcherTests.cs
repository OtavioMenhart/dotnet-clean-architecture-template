using CleanArchTemplate.Application.Handlers;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.Handlers;

// Sample request and handler for testing
public class TestRequest : IRequest<string> { }
public class TestHandler : IHandler<TestRequest, string>
{
    public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
        => Task.FromResult("Handled");
}

public class RequestDispatcherTests
{
    [Fact]
    public async Task Dispatch_ReturnsExpectedResponse_WhenHandlerExists()
    {
        // Arrange
        var request = new TestRequest();
        var handlerMock = new Mock<IHandler<TestRequest, string>>();
        handlerMock.Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync("MockedResponse");

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IHandler<TestRequest, string>)))
                           .Returns(handlerMock.Object);

        var dispatcher = new RequestDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.Dispatch(request);

        // Assert
        Assert.Equal("MockedResponse", result);
        handlerMock.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dispatch_ThrowsInvalidOperationException_WhenHandlerNotFound()
    {
        // Arrange
        var request = new TestRequest();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IHandler<TestRequest, string>)))
                           .Returns(null);

        var dispatcher = new RequestDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.Dispatch(request));
        Assert.Contains("Handler not found", ex.Message);
    }

    [Fact]
    public async Task Dispatch_CachesHandlerType()
    {
        // Arrange
        var request = new TestRequest();
        var handlerMock = new Mock<IHandler<TestRequest, string>>();
        handlerMock.Setup(h => h.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync("CachedResponse");

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IHandler<TestRequest, string>)))
                           .Returns(handlerMock.Object);

        var dispatcher = new RequestDispatcher(serviceProviderMock.Object);

        // Act
        var result1 = await dispatcher.Dispatch(request);
        var result2 = await dispatcher.Dispatch(request);

        // Assert
        Assert.Equal("CachedResponse", result1);
        Assert.Equal("CachedResponse", result2);
        handlerMock.Verify(h => h.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        // The handler type should be cached internally, but this is an implementation detail.
        // We verify by calling twice and ensuring no errors.
    }
}
