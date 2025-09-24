using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductHandler : IHandler<UpdateProductCommand, ProductOutput>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork, ILogger<UpdateProductHandler> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProductOutput> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", command.Id);
                return null;
            }

            product.ChangeName(command.Input.Name);
            product.ChangeUnitPrice(command.Input.UnitPrice);

            _productRepository.Update(product);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Product with ID {ProductId} updated successfully.", command.Id);
            return ProductOutput.FromProductDomain(product);
        }
    }
}
