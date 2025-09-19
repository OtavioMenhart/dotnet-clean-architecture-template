using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;

namespace CleanArchTemplate.Application.UseCases.Product.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductOutput>
    {
        public CreateProductInput Input { get; }
        // Add other properties or methods if needed

        public CreateProductCommand(CreateProductInput input)
        {
            Input = input;
        }
    }
}
