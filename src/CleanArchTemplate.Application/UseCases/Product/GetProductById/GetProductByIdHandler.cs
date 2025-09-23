using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.GetProductById
{
    public class GetProductByIdHandler : IHandler<GetProductByIdQuery, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;

        public GetProductByIdHandler(IBaseRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductOutput> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            return product != null ? ProductOutput.FromProductDomain(product) : null;
        }
    }
}
