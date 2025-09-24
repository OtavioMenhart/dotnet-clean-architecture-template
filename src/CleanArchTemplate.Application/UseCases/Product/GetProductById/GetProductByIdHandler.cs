using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Application.UseCases.Product.GetProductById
{
    public class GetProductByIdHandler : IHandler<GetProductByIdQuery, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly ILogger<GetProductByIdHandler> _logger;

        public GetProductByIdHandler(IBaseRepository<ProductEntity> productRepository, ILogger<GetProductByIdHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductOutput> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", request.Id);
                return null;
            }

            _logger.LogInformation("Retrieved product with ID {ProductId}.", request.Id);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
