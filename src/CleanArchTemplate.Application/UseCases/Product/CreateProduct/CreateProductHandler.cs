using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.CreateProduct
{
    public class CreateProductHandler : IHandler<CreateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;

        public CreateProductHandler(IBaseRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductOutput> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductEntity(request.Input.Name, request.Input.UnitPrice);

            await _productRepository.AddAsync(product);

            return ProductOutput.FromProductDomain(product);
        }
    }
}
