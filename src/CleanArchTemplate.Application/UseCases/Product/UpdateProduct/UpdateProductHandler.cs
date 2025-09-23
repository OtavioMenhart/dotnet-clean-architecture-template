using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductHandler : IHandler<UpdateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;

        public UpdateProductHandler(IBaseRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductOutput> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(command.Id);
            if (product == null) return null;

            product.ChangeName(command.Input.Name);
            product.ChangeUnitPrice(command.Input.UnitPrice);

            await _productRepository.UpdateAsync(product);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
