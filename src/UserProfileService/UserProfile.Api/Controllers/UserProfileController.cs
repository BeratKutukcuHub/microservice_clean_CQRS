using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserProfileService.Application.Commands.CreateUserProfile;
using UserProfileService.Application.DTOs;
using UserProfileService.Application.Queries.GetUserProfile;
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
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
        {
            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
