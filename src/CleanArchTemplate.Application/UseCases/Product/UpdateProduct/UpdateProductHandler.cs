using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductHandler : IHandler<UpdateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductOutput> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
            if (product == null) return null;

            product.ChangeName(command.Input.Name);
            product.ChangeUnitPrice(command.Input.UnitPrice);

            _productRepository.Update(product);
            await _unitOfWork.CommitAsync(cancellationToken);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
