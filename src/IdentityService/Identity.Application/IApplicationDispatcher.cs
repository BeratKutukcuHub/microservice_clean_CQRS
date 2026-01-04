using AbstractionBlocks.Common.Domain;
using IdentityService.Identity.Domain;

namespace IdentityService.Application
{
    public interface IApplicationDispatcher
    {
        Task Dispatch(IEnumerable<IEventDomain> command);
    }
}