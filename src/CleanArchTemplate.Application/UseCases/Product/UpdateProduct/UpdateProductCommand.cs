using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;

namespace CleanArchTemplate.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductCommand : IRequest<ProductOutput>
    {
        public Guid Id { get; }
        public UpdateProductInput Input { get; }
        // Add other properties or methods if needed

        public UpdateProductCommand(Guid id, UpdateProductInput input)
        {
            Id = id;
            Input = input;
        }
    }
}
