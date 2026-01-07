using MailNotification.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace MailNotification.Api.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : ControllerBase
    {
        private readonly ISender _sender;
        public MailController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<ActionResult<Guid>> SendMail(SendMailCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}
