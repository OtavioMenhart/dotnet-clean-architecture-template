using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Domain.Entities;
using CleanArchTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Application.UseCases.Product.DeleteProduct;

public class DeleteProductHandler : IHandler<DeleteProductCommand, bool>
{
    private readonly IBaseRepository<ProductEntity> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(IBaseRepository<ProductEntity> productRepository, IUnitOfWork unitOfWork, ILogger<DeleteProductHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await _productRepository.ExistsAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (!exists) return false;

        await _productRepository.DeleteAsync(request.Id, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Product deleted with ID: {ProductId}", request.Id);
        return true;
    }
}
