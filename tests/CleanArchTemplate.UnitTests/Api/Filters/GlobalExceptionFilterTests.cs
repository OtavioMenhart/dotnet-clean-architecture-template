using CleanArchTemplate.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Api.Filters;

public class GlobalExceptionFilterTests
{
    [Fact]
    public void OnException_LogsErrorAndSetsProblemDetails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GlobalExceptionFilter>>();
        var filter = new GlobalExceptionFilter(loggerMock.Object);

        var exception = new InvalidOperationException("Test exception");
        var context = new ExceptionContext(
            new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            },
            new List<IFilterMetadata>())
        {
            Exception = exception
        };

        // Act
        filter.OnException(context);

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled exception occurred.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        var result = Assert.IsType<ObjectResult>(context.Result);
        var problemDetails = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("An unexpected error occurred.", problemDetails.Title);
        Assert.Equal("Test exception", problemDetails.Detail);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }
}
