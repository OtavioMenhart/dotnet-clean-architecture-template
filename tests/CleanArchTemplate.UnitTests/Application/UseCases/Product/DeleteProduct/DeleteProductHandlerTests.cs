using AutoFixture;
using CleanArchTemplate.Application.UseCases.Product.DeleteProduct;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.UseCases.Product.DeleteProduct
{
    public class DeleteProductHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IBaseRepository<ProductEntity>> _productRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<DeleteProductHandler>> _loggerMock = new();

        [Fact]
        public async Task Handle_ProductExists_DeletesAndCommitsAndLogs_ReturnsTrue()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var command = new DeleteProductCommand(id);

            _productRepositoryMock
                .Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new DeleteProductHandler(
                _productRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _productRepositoryMock.Verify(r => r.ExistsAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var command = new DeleteProductCommand(id);

            _productRepositoryMock
                .Setup(r => r.ExistsAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new DeleteProductHandler(
                _productRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _productRepositoryMock.Verify(r => r.ExistsAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}