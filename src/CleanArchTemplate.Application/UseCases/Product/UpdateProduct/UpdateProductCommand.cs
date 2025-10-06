using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;

namespace CleanArchTemplate.Application.UseCases.Product.UpdateProduct;

public record UpdateProductCommand(Guid Id, UpdateProductInput Input) : IRequest<ProductOutput>;
