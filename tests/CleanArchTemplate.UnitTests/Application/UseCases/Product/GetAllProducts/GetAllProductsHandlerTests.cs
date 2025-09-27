using AutoFixture;
using CleanArchTemplate.Application.UseCases.Product.GetAllProducts;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.UseCases.Product.GetAllProducts
{
    public class GetAllProductsHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IBaseRepository<ProductEntity>> _productRepositoryMock = new();
        private readonly Mock<ILogger<GetAllProductsHandler>> _loggerMock = new();

        [Fact]
        public async Task Handle_ProductsExist_ReturnsMappedOutputAndLogsInformation()
        {
            // Arrange
            var pageNumber = 2;
            var pageSize = 5;
            var totalCount = 10;
            var products = _fixture.CreateMany<ProductEntity>(pageSize).ToList();
            var query = new GetAllProductsQuery(pageNumber, pageSize);

            _productRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalCount);

            _productRepositoryMock
                .Setup(r => r.GetPagedAsync(pageNumber, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            var handler = new GetAllProductsHandler(
                _productRepositoryMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(products.Count, result.Products.Count());
            foreach (var product in products)
            {
                Assert.Contains(result.Products, p => p.Id == product.Id && p.Name == product.Name && p.UnitPrice == product.UnitPrice);
            }
            _productRepositoryMock.Verify(r => r.GetPagedAsync(pageNumber, pageSize, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoProductsExist_ReturnsEmptyOutputAndLogsWarning()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var query = new GetAllProductsQuery(pageNumber, pageSize);

            _productRepositoryMock
                .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var handler = new GetAllProductsHandler(
                _productRepositoryMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Products);
            _productRepositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}