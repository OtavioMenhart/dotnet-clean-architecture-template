using CleanArchTemplate.Domain.Entities;

namespace CleanArchTemplate.Application.UseCases.Product.Common
{
    public class ProductOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static ProductOutput FromProductDomain(ProductEntity product)
        {
            return new ProductOutput
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
