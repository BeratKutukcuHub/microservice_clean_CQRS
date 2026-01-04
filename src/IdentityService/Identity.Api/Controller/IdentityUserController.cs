using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Auth.Identity.Queries;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Security;

namespace IdentityService.Application.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityUserController : ControllerBase
    {
        private readonly ISender _sender;

        public IdentityUserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Guid>> Register(CreateIdentityCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [Authorize(Policy = "IsBlocked")]
        [HttpPut("Update")]
        [Authorize]
        public async Task<ActionResult<UpdateIdentityResponse>> Update(UpdateIdentityCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("Delete/{Id}")]
        [HasPermission("User.Delete")]
        public async Task<ActionResult<bool>> Delete(Guid Id)
        {
            var result = await _sender.Send(new DeleteIdentityUserCommand(Id));
            return Ok(result);
        }

        [HttpGet("GetById/{Id}")]
        [Authorize]
        public async Task<ActionResult<IdentityUserDto>> GetById(Guid Id)
        {
            var result = await _sender.Send(new GetByIdIdentityCommand(Id));
            return Ok(result);
        }

        [HttpGet("GetAll")]
        [HasPermission("User.ViewAll")]
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
        [HttpPost("AssignRole")]
        [HasPermission("User.Update")]
        public async Task<ActionResult<bool>> AssignRole(AssignRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}

