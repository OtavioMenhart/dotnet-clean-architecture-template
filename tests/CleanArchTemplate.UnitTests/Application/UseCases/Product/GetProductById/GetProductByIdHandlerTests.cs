using AutoFixture;
using CleanArchTemplate.Application.UseCases.Product.GetProductById;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.UseCases.Product.GetProductById
{
    public class GetProductByIdHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IBaseRepository<ProductEntity>> _productRepositoryMock = new();
        private readonly Mock<ILogger<GetProductByIdHandler>> _loggerMock = new();

        [Fact]
        public async Task Handle_ProductExists_ReturnsMappedOutputAndLogsInformation()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var product = _fixture.Build<ProductEntity>()
                                  .Create();
            var query = new GetProductByIdQuery(id);

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new GetProductByIdHandler(
                _productRepositoryMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.UnitPrice, result.UnitPrice);
            Assert.Equal(product.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ReturnsNullAndLogsWarning()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var query = new GetProductByIdQuery(id);

            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductEntity)null);

            var handler = new GetProductByIdHandler(
                _productRepositoryMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}