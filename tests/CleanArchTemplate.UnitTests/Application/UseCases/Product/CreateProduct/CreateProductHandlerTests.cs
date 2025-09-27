using AutoFixture;
using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchTemplate.UnitTests.Application.UseCases.Product.CreateProduct
{
    public class CreateProductHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IBaseRepository<ProductEntity>> _productRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<CreateProductHandler>> _loggerMock = new();

        [Fact]
        public async Task Handle_CreatesProduct_AddsToRepository_CommitsUnitOfWork_LogsAndReturnsOutput()
        {
            // Arrange
            var input = _fixture.Create<CreateProductInput>();
            var command = new CreateProductCommand(input);

            ProductEntity? addedProduct = null;
            _productRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ProductEntity>(), It.IsAny<CancellationToken>()))
                .Callback<ProductEntity, CancellationToken>((p, _) => addedProduct = p)
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new CreateProductHandler(
                _productRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(input.Name, addedProduct.Name);
            Assert.Equal(input.UnitPrice, addedProduct.UnitPrice);

            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ProductEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(addedProduct.Id, result.Id);
            Assert.Equal(addedProduct.Name, result.Name);
            Assert.Equal(addedProduct.UnitPrice, result.UnitPrice);
            Assert.Equal(addedProduct.CreatedAt, result.CreatedAt);
        }
    }
}