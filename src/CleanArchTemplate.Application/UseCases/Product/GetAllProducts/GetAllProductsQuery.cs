using CleanArchTemplate.Application.Handlers;

namespace CleanArchTemplate.Application.UseCases.Product.GetAllProducts;

public record GetAllProductsQuery(int PageNumber, int PageSize) : IRequest<GetAllProductsOutput>;
