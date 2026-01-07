using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Auth.Role.Commands;
using IdentityService.Application.Auth.Role.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication.Security;
namespace IdentityService.Application.Api.Controller
{
    [ApiController]
    [Route("api/role")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;
        public RoleController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        [HasPermission("Role.Create")]
        public async Task<ActionResult<RoleDto>> Create(CreateRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpGet]
        [HasPermission("Role.ViewAll")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var result = await _sender.Send(new GetAllRolesQuery());
            return Ok(result);
        }
        [HttpGet("{id}")]
        [HasPermission("Role.ViewAll")]
        public async Task<ActionResult<RoleDto>> GetById(Guid id)
        {
            var result = await _sender.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }
        [HttpPost("{id}/permissions")]
        [HasPermission("Role.Update")]
        public async Task<ActionResult<bool>> AddPermission(Guid id, AddPermissionToRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}/permissions")]
        [HasPermission("Role.Update")]
        public async Task<ActionResult<bool>> RemovePermission(Guid id, RemovePermissionFromRoleCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        [HasPermission("Role.Delete")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteRoleCommand(id));
            return Ok(result);
        }
    }
}
