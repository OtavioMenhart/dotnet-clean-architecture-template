using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Application.UseCases.Product.CreateProduct
{
    public class CreateProductHandler : IHandler<CreateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork, ILogger<CreateProductHandler> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProductOutput> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductEntity(request.Input.Name, request.Input.UnitPrice);
            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
