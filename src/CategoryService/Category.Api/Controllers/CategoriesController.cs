using AbstractionBlocks.Common.Pagination;
using Category.Application.UseCases.Categories.Commands.CreateCategory;
using Category.Application.UseCases.Categories.Commands.UpdateCategory;
using Category.Application.UseCases.Categories.Commands.DeleteCategory;
using Category.Application.UseCases.Categories.Queries.GetAllCategories;
using Category.Application.UseCases.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Category.Api.Controllers;
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationValue
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var query = new GetAllCategoriesQuery(pagination);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}