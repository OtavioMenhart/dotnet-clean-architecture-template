using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;

namespace CleanArchTemplate.Application.UseCases.Product.CreateProduct;

public record CreateProductCommand(CreateProductInput Input) : IRequest<ProductOutput>;
