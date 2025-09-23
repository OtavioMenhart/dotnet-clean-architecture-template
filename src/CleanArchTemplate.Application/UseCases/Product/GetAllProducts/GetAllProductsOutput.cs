using CleanArchTemplate.Application.UseCases.Product.Common;

namespace CleanArchTemplate.Application.UseCases.Product.GetAllProducts
{
    public class GetAllProductsOutput
    {
        public IEnumerable<ProductOutput> Products { get; set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public GetAllProductsOutput(IEnumerable<ProductOutput> products, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = products.Count();
            Products = products;
        }
    }
}
