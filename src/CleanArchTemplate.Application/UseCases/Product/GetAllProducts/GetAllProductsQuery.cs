using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;

namespace CleanArchTemplate.Application.UseCases.Product.GetAllProducts
{
    public record GetAllProductsQuery(int PageNumber, int PageSize) : IRequest<GetAllProductsOutput>;
}
