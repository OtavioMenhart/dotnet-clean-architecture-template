using AutoFixture;
using CleanArchTemplate.Application.UseCases.Product.UpdateProduct;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.UseCases.Product.UpdateProduct;

public class UpdateProductHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBaseRepository<ProductEntity>> _productRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<UpdateProductHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_ProductExists_UpdatesAndCommitsAndLogs_ReturnsMappedOutput()
    {
        // Arrange
        var input = _fixture.Create<UpdateProductInput>();
        var product = new ProductEntity("OldName", 1.0);
        var command = new UpdateProductCommand(product.Id, input);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _productRepositoryMock
            .Setup(r => r.Update(product));

        var handler = new UpdateProductHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.UnitPrice, result.UnitPrice);

        _productRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(r => r.Update(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductDoesNotExist_LogsWarningAndReturnsNull()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var input = _fixture.Create<UpdateProductInput>();
        var command = new UpdateProductCommand(id, input);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductEntity)null);

        var handler = new UpdateProductHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _productRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _productRepositoryMock.Verify(r => r.Update(It.IsAny<ProductEntity>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
