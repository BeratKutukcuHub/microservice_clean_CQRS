using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserProfileService.Application.Commands.CreateUserProfile;
using UserProfileService.Application.Commands.UpdateUserProfile;
using UserProfileService.Application.Commands.DeleteUserProfile;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Queries.GetUserProfile;
using UserProfileService.Application.Queries.GetAllUserProfiles;
using UserProfileService.Application.Queries.SearchUserProfiles;

namespace UserProfileService.Api.Controllers
{
    [ApiController]
    [Route("api/userprofile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<UserProfileDto>> CreateUserProfile([FromBody] CreateUserProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserProfileDto>> UpdateUserProfile(Guid userId, [FromBody] UpdateUserProfileCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("UserId mismatch");
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<bool>> DeleteUserProfile(Guid userId)
        {
            var command = new DeleteUserProfileCommand(userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
        {
            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUserProfiles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var query = new GetAllUserProfilesQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchUserProfiles([FromQuery] string term)
        {
            var query = new SearchUserProfilesQuery(term);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
