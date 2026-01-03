using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Auth.Role.Commands;
using IdentityService.Application.Auth.Role.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Security;

namespace IdentityService.Application.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;

        public RoleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("Create")]
        [HasPermission("Role.Create")]
        public async Task<ActionResult<RoleDto>> Create(CreateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        [HasPermission("Role.ViewAll")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllRolesQuery());
            return Ok(result);
        }

        [HttpGet("GetById/{Id}")]
        [HasPermission("Role.ViewAll")]
        public async Task<ActionResult<RoleDto>> GetById(Guid Id)
        {
            var result = await _sender.Send(new GetRoleByIdQuery(Id));
            return Ok(result);
        }

        [HttpPost("AddPermission")]
        [HasPermission("Role.Update")]
        public async Task<ActionResult<bool>> AddPermission(AddPermissionToRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("RemovePermission")]
        [HasPermission("Role.Update")]
        public async Task<ActionResult<bool>> RemovePermission(RemovePermissionFromRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("Delete/{Id}")]
        [HasPermission("Role.Delete")]
        public async Task<ActionResult<bool>> Delete(Guid Id)
        {
            var result = await _sender.Send(new DeleteRoleCommand(Id));
            return Ok(result);
        }
    }
}

