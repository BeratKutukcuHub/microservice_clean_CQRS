using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Product.Application.Commands;
using ProductService.Product.Application.Queries;
using AbstractionBlocks.Common.Pagination;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;
        public ProductController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPut("Update")]
        public async Task<ActionResult<Guid>> Update(UpdateProductCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpDelete("Delete/{Id}")]
        public async Task<ActionResult<bool>> Delete(Guid Id)
        {
            var result = await _sender.Send(new DeleteProductCommand(Id));
            return Ok(result);
        }
        [HttpPatch("Activate/{Id}")]
        public async Task<ActionResult<bool>> Activate(Guid Id)
        {
            var result = await _sender.Send(new ActivateProductCommand(Id));
            return Ok(result);
        }
        [HttpPatch("Deactivate/{Id}")]
        public async Task<ActionResult<bool>> Deactivate(Guid Id)
        {
            var result = await _sender.Send(new DeactivateProductCommand(Id));
            return Ok(result);
        }
        [HttpGet("GetById/{Id}")]
        public async Task<ActionResult<ProductEntity>> GetById(Guid Id)
        {
            var result = await _sender.Send(new GetProductByIdQuery(Id));
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<PaginationResponse<ProductEntity>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var query = new GetAllProductsQuery(pageNumber, pageSize);
            var result = await _sender.Send(query);
            return Ok(result);
        }
        [HttpGet("GetActive")]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetActive()
        {
            var result = await _sender.Send(new GetActiveProductsQuery());
            return Ok(result);
        }
    }
}
