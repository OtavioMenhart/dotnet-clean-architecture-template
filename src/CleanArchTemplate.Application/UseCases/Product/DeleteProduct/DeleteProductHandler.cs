using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.DeleteProduct
{
    public class DeleteProductHandler : IHandler<DeleteProductCommand, bool>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var exists = await _productRepository.ExistsAsync(request.Id, cancellationToken);
            if (!exists) return false;

            await _productRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
    }
}
