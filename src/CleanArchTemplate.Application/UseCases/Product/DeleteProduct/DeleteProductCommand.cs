using CleanArchTemplate.Application.Handlers;

namespace CleanArchTemplate.Application.UseCases.Product.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
}
