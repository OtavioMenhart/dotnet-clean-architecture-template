using CleanArchTemplate.Api.Responses;
using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Product.Common;
using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Application.UseCases.Product.DeleteProduct;
using CleanArchTemplate.Application.UseCases.Product.GetAllProducts;
using CleanArchTemplate.Application.UseCases.Product.GetProductById;
using CleanArchTemplate.Application.UseCases.Product.UpdateProduct;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IHandler<CreateProductCommand, ProductOutput> _createProductHandler;
    private readonly IHandler<GetProductByIdQuery, ProductOutput> _getProductByIdHandler;
    private readonly IHandler<GetAllProductsQuery, GetAllProductsOutput> _getAllProductsHandler;
    private readonly IHandler<DeleteProductCommand, bool> _deleteProductHandler;
    private readonly IHandler<UpdateProductCommand, ProductOutput> _updateProductHandler;

    public ProductController(IHandler<CreateProductCommand, ProductOutput> createProductHandler,
        IHandler<GetProductByIdQuery, ProductOutput> getProductByIdHandler,
        IHandler<GetAllProductsQuery, GetAllProductsOutput> getAllProductsHandler,
        IHandler<DeleteProductCommand, bool> deleteProductHandler,
        IHandler<UpdateProductCommand, ProductOutput> updateProductHandler)
    {
        _createProductHandler = createProductHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _getAllProductsHandler = getAllProductsHandler;
        _deleteProductHandler = deleteProductHandler;
        _updateProductHandler = updateProductHandler;
    }

    /// <summary>
    /// Creates a new product based on the provided input data.
    /// </summary>
    /// <remarks>This method handles the creation of a product by processing the provided input and
    /// returning the result.  The response includes the created product details in a successful scenario or an
    /// error response in case of failure.</remarks>
    /// <param name="input">The input data required to create the product. This includes details such as the product name and 
    /// unit price.</param>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with the created product details
    /// if the operation is successful. Returns a <see cref="ProblemDetails"/> response if an internal server error
    /// occurs.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductInput input)
    {
        var command = new CreateProductCommand(input);
        var result = await _createProductHandler.Handle(command, CancellationToken.None);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, new ApiResponse<ProductOutput>(result));
    }

    /// <summary>
    /// Gets a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product's unique identifier.</param>
    /// <returns>An ApiResponse with the product details, or NotFound if not found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

        if (result == null)
            return NotFound();

        return Ok(new ApiResponse<ProductOutput>(result));
    }

    /// <summary>
    /// Retrieves a paginated list of all products.
    /// </summary>
    /// <remarks>This method supports pagination through the <paramref name="pageNumber"/> and
    /// <paramref name="pageSize"/> parameters. If no products are found for the specified page, a 404 (Not Found)
    /// response is returned.</remarks>
    /// <param name="pageNumber">The page number to retrieve. Must be 1 or greater. Defaults to 1.</param>
    /// <param name="pageSize">The number of products per page. Must be 1 or greater. Defaults to 10.</param>
    /// <returns>An <see cref="IActionResult"/> containing a paginated list of products wrapped in an  <see
    /// cref="ApiResponseList{T}"/> with a status code of 200 (OK) if products are found. Returns a <see
    /// cref="ProblemDetails"/> with a status code of 404 (Not Found) if no products are available. Returns a <see
    /// cref="ProblemDetails"/> with a status code of 500 (Internal Server Error) in case of an unexpected error.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<ProductOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllProductsQuery(pageNumber, pageSize);
        var products = await _getAllProductsHandler.Handle(query, CancellationToken.None);

        if (products == null || products.Products == null || !products.Products.Any())
            return NotFound();

        var pagedResponse = new ApiResponseList<ProductOutput>(
            products.Products,
            products.PageNumber,
            products.PageSize,
            products.TotalCount);
        return Ok(pagedResponse);
    }

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product's unique identifier.</param>
    /// <returns>NoContent if deleted, NotFound if not found, or ProblemDetails if error.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand(id);
        var deleted = await _deleteProductHandler.Handle(command, CancellationToken.None);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Updates an existing product's information.
    /// </summary>
    /// <param name="id">The product's unique identifier.</param>
    /// <param name="input">The updated product data.</param>
    /// <returns>An ApiResponse with the updated product details, or NotFound if not found.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductInput input)
    {
        var command = new UpdateProductCommand(id, input);
        var result = await _updateProductHandler.Handle(command, CancellationToken.None);

        if (result == null)
            return NotFound();

        return Ok(new ApiResponse<ProductOutput>(result));
    }
}
