using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;

namespace CleanArchTemplate.Application.UseCases.Product.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductOutput>;
}
