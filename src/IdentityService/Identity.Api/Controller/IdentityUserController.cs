using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Auth.Identity.Queries;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Security;
using IdentityService.Identity.Application.Auth.Identity.Commands;
namespace IdentityService.Application.Api.Controller
{
    [ApiController]
    [Route("api/identityuser")]
    public class IdentityUserController : ControllerBase
    {
        private readonly ISender _sender;
        public IdentityUserController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register(CreateIdentityCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [Authorize(Policy = "IsBlocked")]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(Guid id, UpdateIdentityCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [HasPermission("User.Delete")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteIdentityUserCommand(id));
            return Ok(result);
        }
        [HttpPatch("{id}/block")]
        [HasPermission("User.Update")]
        public async Task<ActionResult<bool>> Block(Guid id)
        {
            var result = await _sender.Send(new BlockUserCommand(id));
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<IdentityUserDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetByIdIdentityCommand(id));
            return Ok(result);
        }
        [HttpGet]
        [HasPermission("User.ViewAll")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<IEnumerable<IdentityUserDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var pagination = new AbstractionBlocks.Common.Pagination.PaginationValue
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var command = new GetAllIdentityUsersCommand(pagination);
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpPost("{id}/roles")]
        [HasPermission("User.Update")]
        public async Task<ActionResult<bool>> AssignRole(Guid id, AssignRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}
