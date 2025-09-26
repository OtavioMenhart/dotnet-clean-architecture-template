using AutoFixture;
using CleanArchTemplate.Api.Controllers;
using CleanArchTemplate.Api.Responses;
using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Application.UseCases.Product.DeleteProduct;
using CleanArchTemplate.Application.UseCases.Product.GetAllProducts;
using CleanArchTemplate.Application.UseCases.Product.GetProductById;
using CleanArchTemplate.Application.UseCases.Product.UpdateProduct;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CleanArchTemplate.UnitTests.Api.Controllers
{
    public class ProductControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IHandler<CreateProductCommand, ProductOutput>> _createProductHandlerMock;
        private readonly Mock<IHandler<GetProductByIdQuery, ProductOutput>> _getProductByIdHandlerMock;
        private readonly Mock<IHandler<GetAllProductsQuery, GetAllProductsOutput>> _getAllProductsHandlerMock;
        private readonly Mock<IHandler<DeleteProductCommand, bool>> _deleteProductHandlerMock;
        private readonly Mock<IHandler<UpdateProductCommand, ProductOutput>> _updateProductHandlerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _fixture = new Fixture();
            _createProductHandlerMock = new Mock<IHandler<CreateProductCommand, ProductOutput>>();
            _getProductByIdHandlerMock = new Mock<IHandler<GetProductByIdQuery, ProductOutput>>();
            _getAllProductsHandlerMock = new Mock<IHandler<GetAllProductsQuery, GetAllProductsOutput>>();
            _deleteProductHandlerMock = new Mock<IHandler<DeleteProductCommand, bool>>();
            _updateProductHandlerMock = new Mock<IHandler<UpdateProductCommand, ProductOutput>>();

            _controller = new ProductController(
                _createProductHandlerMock.Object,
                _getProductByIdHandlerMock.Object,
                _getAllProductsHandlerMock.Object,
                _deleteProductHandlerMock.Object,
                _updateProductHandlerMock.Object
            );
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction_WithApiResponse()
        {
            var input = _fixture.Create<CreateProductInput>();
            var output = _fixture.Create<ProductOutput>();
            _createProductHandlerMock
                .Setup(h => h.Handle(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(output);

            var result = await _controller.CreateProduct(input);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ProductOutput>>(createdResult.Value);
            Assert.Equal(output, apiResponse.Data);
            Assert.Equal(nameof(_controller.GetProductById), createdResult.ActionName);
            Assert.Equal(output.Id, (dynamic)createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenProductExists()
        {
            var id = Guid.NewGuid();
            var output = _fixture.Create<ProductOutput>();
            _getProductByIdHandlerMock
                .Setup(h => h.Handle(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(output);

            var result = await _controller.GetProductById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ProductOutput>>(okResult.Value);
            Assert.Equal(output, apiResponse.Data);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var id = Guid.NewGuid();
            _getProductByIdHandlerMock
                .Setup(h => h.Handle(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductOutput)null);

            var result = await _controller.GetProductById(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WhenProductsExist()
        {
            var products = _fixture.CreateMany<ProductOutput>(3).ToList();
            var output = new GetAllProductsOutput(products, 1, 10, products.Count);
            _getAllProductsHandlerMock
                .Setup(h => h.Handle(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(output);

            var result = await _controller.GetAllProducts(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponseList = Assert.IsType<ApiResponseList<ProductOutput>>(okResult.Value);
            Assert.Equal(products, apiResponseList.Data);
            Assert.Equal(1, apiResponseList.PageNumber);
            Assert.Equal(10, apiResponseList.PageSize);
            Assert.Equal(products.Count, apiResponseList.TotalCount);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsNotFound_WhenNoProductsExist()
        {
            var output = new GetAllProductsOutput(new List<ProductOutput>(), 1, 10, 0);
            _getAllProductsHandlerMock
                .Setup(h => h.Handle(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(output);

            var result = await _controller.GetAllProducts(1, 10);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _deleteProductHandlerMock
                .Setup(h => h.Handle(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.DeleteProduct(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotDeleted()
        {
            var id = Guid.NewGuid();
            _deleteProductHandlerMock
                .Setup(h => h.Handle(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.DeleteProduct(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOk_WhenProductUpdated()
        {
            var id = Guid.NewGuid();
            var input = _fixture.Create<UpdateProductInput>();
            var output = _fixture.Create<ProductOutput>();
            _updateProductHandlerMock
                .Setup(h => h.Handle(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(output);

            var result = await _controller.UpdateProduct(id, input);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ProductOutput>>(okResult.Value);
            Assert.Equal(output, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductNotUpdated()
        {
            var id = Guid.NewGuid();
            var input = _fixture.Create<UpdateProductInput>();
            _updateProductHandlerMock
                .Setup(h => h.Handle(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductOutput)null);

            var result = await _controller.UpdateProduct(id, input);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
