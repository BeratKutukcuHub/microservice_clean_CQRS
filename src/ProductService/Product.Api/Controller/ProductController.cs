using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Product.Application.Commands;
using ProductService.Product.Application.Queries;
using AbstractionBlocks.Common.Pagination;
using ProductEntity = ProductService.Product.Domain.Product;

namespace ProductService.Product.Api.Controller
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Guid>> Update(Guid id, UpdateProductCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteProductCommand(id));
            return Ok(result);
        }

        [HttpPatch("{id}/activate")]
        public async Task<ActionResult<bool>> Activate(Guid id)
        {
            var result = await _sender.Send(new ActivateProductCommand(id));
            return Ok(result);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult<bool>> Deactivate(Guid id)
        {
            var result = await _sender.Send(new DeactivateProductCommand(id));
            return Ok(result);
        }

        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<bool>> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
        {
            var result = await _sender.Send(new UpdateStockCommand(id, request.Quantity));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductEntity>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetProductByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<ProductEntity>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var query = new GetAllProductsQuery(pageNumber, pageSize);
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetActive()
        {
            var result = await _sender.Send(new GetActiveProductsQuery());
            return Ok(result);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult> GetByCategory(string category)
        {
            var result = await _sender.Send(new GetProductsByCategoryQuery(category));
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult> Search([FromQuery] string term)
        {
            var result = await _sender.Send(new SearchProductsQuery(term));
            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            var result = await _sender.Send(new GetLowStockProductsQuery(threshold));
            return Ok(result);
        }
    }

    public record UpdateStockRequest(int Quantity);
}
