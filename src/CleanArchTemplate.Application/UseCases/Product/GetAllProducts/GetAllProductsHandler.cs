using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Application.UseCases.Product.GetAllProducts
{
    public class GetAllProductsHandler : IHandler<GetAllProductsQuery, GetAllProductsOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly ILogger<GetAllProductsHandler> _logger;

        public GetAllProductsHandler(IBaseRepository<ProductEntity> productRepository, ILogger<GetAllProductsHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<GetAllProductsOutput> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var totalProducts = await _productRepository.CountAsync(cancellationToken);
            if (totalProducts == 0)
            {
                _logger.LogWarning("No products found in the repository.");
                return new GetAllProductsOutput(Enumerable.Empty<ProductOutput>(), request.PageNumber, request.PageSize, 0);
            }

            var products = await _productRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
            _logger.LogInformation("Retrieved {ProductCount} products for page {PageNumber} with page size {PageSize}.", products.Count(), request.PageNumber, request.PageSize);
            return new GetAllProductsOutput(products.Select(ProductOutput.FromProductDomain), request.PageNumber, request.PageSize, totalProducts);
        }
    }
}
