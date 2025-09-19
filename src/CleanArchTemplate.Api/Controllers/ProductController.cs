using CleanArchTemplate.Api.Responses;
using CleanArchTemplate.Application.Handlers;
using CleanArchTemplate.Application.UseCases.Common;
using CleanArchTemplate.Application.UseCases.Product.CreateProduct;
using CleanArchTemplate.Application.UseCases.Product.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IHandler<CreateProductCommand, ProductOutput> _createProductHandler;
        private readonly IHandler<GetProductByIdQuery, ProductOutput> _getProductByIdHandler;

        public ProductController(IHandler<CreateProductCommand, ProductOutput> createProductHandler,
            IHandler<GetProductByIdQuery, ProductOutput> getProductByIdHandler)
        {
            _createProductHandler = createProductHandler;
            _getProductByIdHandler = getProductByIdHandler;
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
        [HttpPost("create-product")]
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
    }
}
