using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;

namespace CleanArchTemplate.Application.UseCases.Product.DeleteProduct
{
    public class DeleteProductHandler : IHandler<DeleteProductCommand, bool>
    {
        private readonly IBaseRepository<ProductEntity> _productRepository;
        public DeleteProductHandler(IBaseRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken = default)
        {
            var exists = await _productRepository.ExistsAsync(request.Id);
            if (!exists) return false;

            await _productRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}
