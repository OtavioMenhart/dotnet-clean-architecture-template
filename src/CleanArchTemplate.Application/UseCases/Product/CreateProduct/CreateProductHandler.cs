using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.CreateProduct
{
    public class CreateProductHandler : IHandler<CreateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductOutput> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductEntity(request.Input.Name, request.Input.UnitPrice);
            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
