using AbstractionBlocks.Common.Domain;

namespace IdentityService.Application.Interfaces
{
    public interface IEventApplicationHandler<T> where T : IEventDomain
    {
        Task Handle(T reqEvent);
    }
}