using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.GetAllProducts
{
    public class GetAllProductsHandler : IHandler<GetAllProductsQuery, GetAllProductsOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;

        public GetAllProductsHandler(IBaseRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetAllProductsOutput> Handle(GetAllProductsQuery request, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetPagedAsync(request.PageNumber, request.PageSize);
            var xx = products.Select(ProductOutput.FromProductDomain);
            return new GetAllProductsOutput(products.Select(ProductOutput.FromProductDomain), request.PageNumber, request.PageSize);
        }
    }
}
